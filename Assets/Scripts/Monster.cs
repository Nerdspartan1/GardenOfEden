using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

public class Monster : MonoBehaviour
{
	public enum AI
	{
		Chase,
		Roam,
		Trap
	}

	public PlayerController Player;

	//VFX
	private GlitchEffect _glitchEffect;
	private Grain _grain;
	private DeadPixelGenerator _deadPixelGenerator;
	private ApocalypseFilter _apocalypseFilter;

	public MapGenerator MapGenerator;
	public PropsManager PropsManager;
	public GameOver GameOver;

	[Header("Roaming Behaviour")]
	public float SightDistance = 18f;
	public LayerMask CantSeeThrough;
	public float TeleportationPeriod = 10f;
	
	[Header("Chase Behaviour")]
	public float QuitChasingDelay = 5f;
	public float LoseSightDistance = 24f;
	public float ChaseTimeBeforeTeleport = 10f;
	public float CatchDistance = 3f;
	public float ConfusionPeriod = 1f;

	[Header("Aggressivity")]
	public float AggressivitySpeedGain = 0.5f;
	public float TeleportationPeriodGain = 2f;

	private Vector3 _lastSeenPlayerPosition;
	private bool _destinationInitialized = false;
	private float _timeBeforeQuitChasing;
	private float _timeBeforeTeleportation;
	private float _timeBeforeConfuseChance;
	private float _baseSpeed;
	private bool _caughtPlayer = false;

	public int Aggressivity;
	public Vector3 CurrentDestination;
	public bool PlayerLineObstructed;
	public AI CurrentAI;

	private NavMeshAgent _nav;

    //Adding fmod entity sound + chase audio

    FMOD.Studio.EventInstance EntityEvent;

    FMOD.Studio.PARAMETER_DESCRIPTION pd;
    FMOD.Studio.PARAMETER_ID EnemyDistID;

    void Awake()
    {
		_nav = GetComponent<NavMeshAgent>();
		_nav.updateRotation = false;
		_glitchEffect = Player.GetComponentInChildren<GlitchEffect>();
		_deadPixelGenerator = Player.GetComponentInChildren<DeadPixelGenerator>();
		_apocalypseFilter = Player.GetComponentInChildren<ApocalypseFilter>();
		_grain = Player.GetComponentInChildren<PostProcessVolume>().sharedProfile.GetSetting<Grain>();
	}

	private void Start()
	{
		_baseSpeed = _nav.speed;
		CurrentAI = AI.Roam;
		Aggressivity = 0;
		_timeBeforeTeleportation = TeleportationPeriod;
		SwitchAppearance();

		EntityEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Enemies/Entity Chase BGM");

        FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("Enemy Dist", out pd);
        EnemyDistID = pd.id;
	}

	void Update()
	{
		if (!_caughtPlayer)
		{
			PlayerLineObstructed = Physics.Linecast(transform.position, Player.transform.position, CantSeeThrough);
			switch (CurrentAI)
			{
				case AI.Chase:
					_nav.SetDestination(Player.transform.position);
					var distanceToPlayer = (transform.position - Player.transform.position).magnitude;
					if (PlayerDistanceInSight < LoseSightDistance)
					{
						_timeBeforeQuitChasing = QuitChasingDelay;
						if (Aggressivity >= 4)
						{
							if (_timeBeforeTeleportation <= 0)//continuously chased the player for a period of time
							{
								Teleport();
								_timeBeforeTeleportation = ChaseTimeBeforeTeleport;
							}
							_timeBeforeTeleportation -= Time.deltaTime;

						}
						if (_timeBeforeConfuseChance <= 0f)
						{
							if (Random.value < 0.015f*Aggressivity)
							{
								Player.Confuse((float)Aggressivity/3f);
							}
							_timeBeforeConfuseChance = ConfusionPeriod;
						}
						_timeBeforeConfuseChance -= Time.deltaTime;
					}
					else
					{
						if (_timeBeforeQuitChasing == QuitChasingDelay)//just lost sight
						{ 
							_lastSeenPlayerPosition = Player.transform.position;
						}
						_timeBeforeQuitChasing -= Time.deltaTime;
						
					}

					
					if (_timeBeforeQuitChasing <= 0) //quit chasing
					{
						_timeBeforeTeleportation = TeleportationPeriod;
						CurrentAI = AI.Roam;
						Teleport();
						_destinationInitialized = false;

						FMODUnity.RuntimeManager.StudioSystem.setParameterByID(EnemyDistID, 0f);

					}

					break;
				case AI.Roam:
					if ((transform.position - CurrentDestination).sqrMagnitude < 1f || !_destinationInitialized) //reached destination
					{
						CurrentDestination = MapGenerator.CellToWorld(MapGenerator.GetRandomCellPositionInLevel());
						_nav.SetDestination(CurrentDestination);
						_destinationInitialized = true;
					}

					if (_timeBeforeTeleportation <= 0f)
					{
						Teleport();
						SwitchAppearance();
						_timeBeforeTeleportation = TeleportationPeriod - Aggressivity*TeleportationPeriodGain;
					}
					_timeBeforeTeleportation -= Time.deltaTime;

					if (PlayerDistanceInSight < SightDistance) //start chase
					{
						_timeBeforeTeleportation = ChaseTimeBeforeTeleport;
						_timeBeforeConfuseChance = ConfusionPeriod;
						_destinationInitialized = false;
						//_timeBeforeTeleportation = TeleportationPeriod;
						CurrentAI = AI.Chase;

						EntityEvent.start();
						FMODUnity.RuntimeManager.StudioSystem.setParameterByID(EnemyDistID, 100f);

					}
					break;
			}

			if (PlayerDistance < CatchDistance)
			{
				_caughtPlayer = true;
				GameOver.enabled = true;

                FMODUnity.RuntimeManager.StudioSystem.setParameterByID(EnemyDistID, 0f);
                EntityEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                EntityEvent.release();
            }
		}

		UpdateEffects();
	}

	public void UpdateEffects()
	{
		float intensityFactor = Mathf.Min(1f, 10f / PlayerDistanceInSight); //max intensity at 10 meter

		float minIntensity = 0.1f * Mathf.Max(0, Aggressivity - 3);
		intensityFactor = Mathf.Max(minIntensity, intensityFactor);
		_glitchEffect.intensity = intensityFactor;
		_glitchEffect.flipIntensity = intensityFactor;
		_glitchEffect.colorIntensity = intensityFactor > 0.2f ? intensityFactor : 0f; //add dead zone here bc even low factor changes color significantly
		_grain.intensity.value = intensityFactor;
		_deadPixelGenerator.Intensity = intensityFactor;

		_apocalypseFilter.enabled = (Aggressivity >= 5) && intensityFactor > 0.5f;
	}

	public void LevelUpAggressivity(int aggressivity)
	{
		Aggressivity = aggressivity;
		_nav.speed = _baseSpeed + aggressivity * AggressivitySpeedGain;
		if(CurrentAI != AI.Chase)
		{
			Teleport();
		}
		RenderSettings.fogColor = 1f * Color.red + (1f - (float)Aggressivity / 6f) * (Color.cyan);

		Player.Camera.backgroundColor = RenderSettings.fogColor;
	}

	public bool Teleport()
	{
		float dist = 15f + Mathf.Max(0f, (6 - Aggressivity) * 5f);
		return Teleport(dist);
	}

	public bool Teleport(float distanceToThePlayer, float deviation = 0.3f)
	{
		Vector3 pos;
		float distance;
		int k = 0;
		do
		{
			pos = MapGenerator.CellToWorld(MapGenerator.GetRandomCellPositionInLevel());
			distance = (Player.transform.position - pos).magnitude;
			if (k++ > 1000) return false; // teleport fail
		} while (Mathf.Abs(1 - distance/distanceToThePlayer) > deviation);
		_nav.Warp(pos);
		_destinationInitialized = false;
		return true;
	}

	public void SwitchAppearance()
	{
		Destroy(transform.GetChild(0).gameObject);
		var prop = Instantiate(PropsManager.GetRandomProp(),transform);
		prop.transform.localPosition = new Vector3(0,-0.56f,0);
		prop.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360f), 0);
		prop.isStatic = false;
		prop.GetComponent<Collider>().enabled = false;
	}

	public float PlayerDistanceInSight
	{
		get => PlayerLineObstructed ? 1000f : PlayerDistance;
	}

	public float PlayerDistance
	{
		get => (transform.position - Player.transform.position).magnitude;
	}
}

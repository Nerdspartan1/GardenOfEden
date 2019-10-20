using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
	public enum AI
	{
		Chase,
		Roam,
		Trap
	}

	public GameObject Player;
	public MapGenerator MapGenerator;
	public PropsManager PropsManager;

	public float SightDistance = 18f;
	public float LoseSightDistance = 24f;
	public LayerMask CantSeeThrough;
	public float QuitChasingDelay = 5f;
	public float TeleportationPeriod = 10f;

	private Vector3 _lastSeenPlayerPosition;
	private bool _destinationInitialized = false;
	private float _timeBeforeQuitChasing;
	private float _timeBeforeTeleportation;
	private float _baseSpeed;

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
		PlayerLineObstructed = Physics.Linecast(transform.position, Player.transform.position,CantSeeThrough);
		switch (CurrentAI) {
			case AI.Chase:
				_nav.SetDestination(Player.transform.position);
				var distanceToPlayer = (transform.position - Player.transform.position).magnitude;
				if (PlayerDistanceInSight < LoseSightDistance)
				{
					_timeBeforeQuitChasing = QuitChasingDelay;
				}
				else
				{
					if (_timeBeforeQuitChasing == QuitChasingDelay) //just lost sight
						_lastSeenPlayerPosition = Player.transform.position;
					_timeBeforeQuitChasing -= Time.deltaTime;
				}

				if (_timeBeforeQuitChasing <= 0) //quit chasing
				{
					CurrentAI = AI.Roam;
					CurrentDestination = _lastSeenPlayerPosition;
					_destinationInitialized = true;

                    FMODUnity.RuntimeManager.StudioSystem.setParameterByID(EnemyDistID, 0f);
                    EntityEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    //EntityEvent.release();

                }

                break;
			case AI.Roam:
				if((transform.position - CurrentDestination).sqrMagnitude < 1f || !_destinationInitialized) //reached destination
				{
					CurrentDestination = MapGenerator.CellToWorld(MapGenerator.GetRandomCellPositionInLevel());
					_nav.SetDestination(CurrentDestination);
					_destinationInitialized = true;
				}
				
				if(_timeBeforeTeleportation <= 0f)
				{
					Teleport();
					SwitchAppearance();
					_timeBeforeTeleportation = TeleportationPeriod;
				}
				_timeBeforeTeleportation -= Time.deltaTime;

				if (PlayerDistanceInSight < SightDistance) //start chase
				{
					_destinationInitialized = false;
					_timeBeforeTeleportation = TeleportationPeriod;
					CurrentAI = AI.Chase;

                    EntityEvent.start();
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByID(EnemyDistID, 100f);
                    
                }
				break;
		}
	}

	public void LevelUpAggressivity(int aggressivity)
	{
		Aggressivity = aggressivity;
		_nav.speed = _baseSpeed + aggressivity * 0.4f;
		if(CurrentAI != AI.Chase)
		{
			Teleport();
		}
		RenderSettings.fogColor = 1f * Color.red + (1f - (float)Aggressivity / 6f) * (Color.cyan);
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

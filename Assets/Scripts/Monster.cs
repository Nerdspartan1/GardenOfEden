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

	public float SightDistance = 18f;
	public float LoseSightDistance = 24f;
	public LayerMask CantSeeThrough;
	public float QuitChasingDelay = 5f;

	private Vector3 _lastSeenPlayerPosition;
	private bool _destinationInitialized = false;
	private float _timeBeforeQuitChasing;
	public Vector3 CurrentDestination;
	public bool PlayerLineObstructed;
	public AI CurrentAI;

	private NavMeshAgent _nav;

	
	


    void Awake()
    {
		_nav = GetComponent<NavMeshAgent>();
		CurrentAI = AI.Roam;
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

				if (_timeBeforeQuitChasing <= 0)
				{
					CurrentAI = AI.Roam;
					CurrentDestination = _lastSeenPlayerPosition;
					_destinationInitialized = true;
				}
					
				break;
			case AI.Roam:
				if((transform.position - CurrentDestination).sqrMagnitude < 1f || !_destinationInitialized) //reached destination
				{
					CurrentDestination = MapGenerator.CellToWorld(MapGenerator.GetRandomCellPositionInLevel());
					_nav.SetDestination(CurrentDestination);
					_destinationInitialized = true;
				}

				if(PlayerDistanceInSight < SightDistance)
				{
					_destinationInitialized = false;
					CurrentAI = AI.Chase;
				}
				break;
		}
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

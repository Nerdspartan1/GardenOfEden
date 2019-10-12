using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
	NavMeshAgent _nav;
	public GameObject Player;

    void Awake()
    {
		_nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
		_nav.SetDestination(Player.transform.position);
    }
}

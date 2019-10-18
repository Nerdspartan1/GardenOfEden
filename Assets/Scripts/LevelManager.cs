using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

	public static LevelManager Instance;
	public Monster Monster;
	public int NumberOfCollectibles;
	[SerializeField]
	private int _numberOfCollectiblesLeft;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		_numberOfCollectiblesLeft = NumberOfCollectibles;
		Monster.Aggressivity = 0;
    }

	public void Collect()
	{
		_numberOfCollectiblesLeft--;
		Monster.LevelUpAggressivity(NumberOfCollectibles - _numberOfCollectiblesLeft);
	}

}

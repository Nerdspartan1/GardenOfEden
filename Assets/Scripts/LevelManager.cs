using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance;
	public int NumberOfCollectibles;
	private int _numberOfCollectiblesLeft;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		_numberOfCollectiblesLeft = NumberOfCollectibles;
    }

	public void Collect()
	{
		_numberOfCollectiblesLeft--;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}

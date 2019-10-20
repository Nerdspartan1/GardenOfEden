using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsManager : MonoBehaviour
{
	public GameObject[] Props;
	public GameObject[] Collectibles;

	public GameObject GetRandomProp()
	{
		return Props[Random.Range(0, Props.Length - 1)];
	}

}

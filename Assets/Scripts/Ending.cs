using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
	public float WallDestructionRate = 128f; //walls per second
	public float TimeBeforeGroundDisappear = 5f;

	private TileGenerator _tileGenerator;

	private void Awake()
	{
		_tileGenerator = GetComponent<TileGenerator>();
	}

	private void OnEnable()
	{
		EraseLevel();
	}

	public void EraseLevel()
	{
		RenderSettings.fog = false;
		StartCoroutine(EraseLevelAnimation());
	}

	public IEnumerator EraseLevelAnimation()
	{

		while (_tileGenerator.WallsParent.childCount > 0)
		{
			var wall = _tileGenerator.WallsParent.GetChild(Random.Range(0, _tileGenerator.WallsParent.childCount - 1));
			Destroy(wall.gameObject);

			float nextDestructionPeriod = 1f / WallDestructionRate;

			yield return new WaitForSeconds(nextDestructionPeriod);
			TimeBeforeGroundDisappear -= nextDestructionPeriod;

			if (TimeBeforeGroundDisappear <= 0)
				Destroy(_tileGenerator.Ground);

		}

		if (TimeBeforeGroundDisappear > 0)
		{
			yield return new WaitForSeconds(TimeBeforeGroundDisappear);
			Destroy(_tileGenerator.Ground);
		}
			

		

	}


}

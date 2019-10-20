using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
	public PropsManager PropsManager;
	public PlayerController Player;
	public Monster Monster;
	public Image Black;
	public Text EndText;

	public GameObject Monolith;
	public GameObject MonolithParticles;

	public float IncantationTime = 20f;
	public float ActivationDistance = 3f;
	public float WallDestructionRate = 128f; //walls per second
	public float TimeBeforeGroundDisappear = 5f;

	private TileGenerator _tileGenerator;
	private bool _endingInitiated = false;

	private void Awake()
	{
		_tileGenerator = GetComponent<TileGenerator>();
		MonolithParticles.SetActive(false);
	}

	private void OnEnable()
	{
		MonolithParticles.SetActive(true);
	}

	private void Update()
	{
		if(!_endingInitiated && (Player.transform.position - Monolith.transform.position).magnitude < ActivationDistance)
		{
			StartCoroutine(IncantationAnimation());
			StartCoroutine(EraseLevelAnimation());
			Monster.gameObject.SetActive(false);
			Monster.transform.position = new Vector3(0, 1000f, 0);
			_endingInitiated = true;
		}
	}

	public IEnumerator IncantationAnimation()
	{
		float monolithDistance = 1f;

		List<GameObject> collectibles = new List<GameObject>();
		for (int i = 0; i < PropsManager.Collectibles.Length; ++i)
		{
			var instance = Instantiate(PropsManager.Collectibles[i]);
			instance.tag = "Untagged";
			instance.GetComponent<Collectible>().enabled = false;

			float angle = 6.28f * (float)i / (float)PropsManager.Collectibles.Length;
			instance.transform.position = Monolith.transform.position + monolithDistance*(Mathf.Cos(angle)*Vector3.forward + Mathf.Sin(angle)*Vector3.right);

			collectibles.Add(instance);
		}

		var spinSpeed = 0.5f;
		while (IncantationTime > 0)
		{
			foreach(var col in collectibles)
			{
				col.transform.RotateAround(Monolith.transform.position, Vector3.up,360f* Time.deltaTime*spinSpeed);
			}
			yield return null;
			IncantationTime -= Time.deltaTime;
		}
	}

	public IEnumerator EraseLevelAnimation()
	{
		//RenderSettings.fogColor = Color.black;
		RenderSettings.fog = false;
		Player.GetComponentInChildren<Camera>().backgroundColor = Color.black;

		while (TimeBeforeGroundDisappear > 0)
		{
			var wall = _tileGenerator.WallsParent.GetChild(Random.Range(0, _tileGenerator.WallsParent.childCount - 1));
			Destroy(wall.gameObject);

			float nextDestructionPeriod = 1f / WallDestructionRate;

			yield return new WaitForSeconds(nextDestructionPeriod);
			TimeBeforeGroundDisappear -= nextDestructionPeriod;

		}

		Destroy(_tileGenerator.Ground);
		Player.GetComponent<FMODFootsteps>().enabled = false;
		StartCoroutine(FadeToBlack());

	}

	public IEnumerator FadeToBlack()
	{
		float timeToFadeOut = 5f;
		while (timeToFadeOut > 0)
		{
			Black.color = new Color(0, 0, 0, 1 - timeToFadeOut / 5f);
		
			timeToFadeOut -= Time.deltaTime;
			yield return null;
		}

		StartCoroutine(ShowEndText());
	}

	public IEnumerator ShowEndText()
	{
		EndText.gameObject.SetActive(true);

		yield return new WaitForSeconds(3f);

		LevelManager.Instance.WriteMessage();

		Application.Quit();
		
	}
}

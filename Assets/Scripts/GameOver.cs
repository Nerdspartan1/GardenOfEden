using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class GameOver : MonoBehaviour
{
	public PlayerController Player;
	public Monster Monster;
	public Image Black;
	public Image CreepyImage;
	public Image BSOD;

	public float BlinkRate = 20f;
	public float AnimationDuration = 1f;

    public LevelManager LevelManager;

	private void OnEnable()
	{
		Player.enabled = false;
		Monster.GetComponent<NavMeshAgent>().enabled = false;

		if (Monster.Aggressivity < 6) StartCoroutine(GameOverAnimation());
		else StartCoroutine(BSODAnimation());
	}

	public IEnumerator BSODAnimation()
	{
		LevelManager.Instance.EverythingbutMenuBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
		Player.GetComponentInChildren<GlitchEffect>().enabled = false;
		FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Enemy Dist", 0f);
		BSOD.enabled = true;

		yield return new WaitForSeconds(4f);

		FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Enemy Dist", 100f);
		Player.GetComponentInChildren<GlitchEffect>().enabled = true;
		StartCoroutine(GameOverAnimation());
	}

	public IEnumerator GameOverAnimation()
	{

		CreepyImage.transform.position = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height));

        FMODUnity.RuntimeManager.PlayOneShot("event:/Enemies/Ending Jumpscare Lose");

        float time = 0f;
		while(time < AnimationDuration)
		{
			Black.color = new Color(0, 0, 0, 1-Black.color.a);
			CreepyImage.color = new Color(1, 1, 1, Black.color.a);
			yield return new WaitForSeconds(1f/BlinkRate);
			time += 1f / BlinkRate;
		}

		LevelManager.Instance.LeaveGame(true);
	}
}

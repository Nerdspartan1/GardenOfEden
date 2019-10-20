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

	public float BlinkRate = 20f;
	public float AnimationDuration = 1f;

	private void OnEnable()
	{
		StartCoroutine(GameOverAnimation());
	}

	public IEnumerator GameOverAnimation()
	{
		Player.enabled = false;
		Monster.GetComponent<NavMeshAgent>().enabled = false;
		CreepyImage.transform.position = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height));

		float time = 0f;
		while(time < AnimationDuration)
		{
			Black.color = new Color(0, 0, 0, 1-Black.color.a);
			CreepyImage.color = new Color(1, 1, 1, Black.color.a);
			yield return new WaitForSeconds(1f/BlinkRate);
			time += 1f / BlinkRate;
		}

		GameManager.Instance.RestartGame();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
	public GameObject Player;

	public float LevitationHeight = 1.8f;
	public float SineAmplitude = 0.2f;
	public float SineFrequency = 1;
	public float SpinFrequency = 0.5f;
	private float _t = 0f;

	public float VibrationIntensity = 0.1f;

	public GameObject EffectOnPickUp;

    void Update()
    {
		//levitate
		transform.position = new Vector3(
			transform.position.x,
			LevitationHeight + SineAmplitude * Mathf.Sin(6.28f * _t * SineFrequency),
			transform.position.z);

		transform.rotation = Quaternion.Euler(0, 360f*_t * SpinFrequency, 0f);
		_t += Time.deltaTime;

		//vibrate when player approaches
		float sqrDistance = (transform.position - Player.transform.position).sqrMagnitude;
		float vibration = VibrationIntensity / sqrDistance;

		transform.position += Random.onUnitSphere * vibration;

    }

	public void PickUp()
	{
		var effect = Instantiate(EffectOnPickUp, transform.position, Quaternion.identity);
		Destroy(effect, 3f);
		Destroy(gameObject);
	}


}

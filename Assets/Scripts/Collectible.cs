using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
	public GameObject Player;
	public CollectiblePopup CollectiblePopup;
	public RenderTexture IconTexture;

	public float LevitationHeight = 1.8f;
	public float SineAmplitude = 0.2f;
	public float SineFrequency = 1;
	public float SpinFrequency = 0.5f;
	private float _t = 0f;

	private Vector3 _initialPosition;

	public float VibrationIntensity = 0.1f;

	public GameObject EffectOnPickUp;

	private void Start()
	{
		_initialPosition = transform.position + LevitationHeight*Vector3.up;
	}

	void Update()
    {
		//levitate
		transform.position = _initialPosition + SineAmplitude * Mathf.Sin(6.28f * _t * SineFrequency) * Vector3.up;

		transform.rotation = Quaternion.Euler(0, 360f*_t * SpinFrequency, 0f);
		_t += Time.deltaTime;

		//vibrate when player approaches
		if (Player)
		{
			float sqrDistance = (transform.position - Player.transform.position).sqrMagnitude;
			float vibration = VibrationIntensity / sqrDistance;

			transform.position += Random.onUnitSphere * vibration;
		}

    }

	public void PickUp()
	{
		var effect = Instantiate(EffectOnPickUp, transform.position, Quaternion.identity);
		CollectiblePopup.Add(IconTexture);
		Destroy(effect, 3f);
		Destroy(gameObject);
	}


}

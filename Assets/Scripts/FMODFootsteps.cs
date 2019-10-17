using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class FMODFootsteps : MonoBehaviour
{
    //adding footsteps sounds through fmod below

    [FMODUnity.EventRef]
    public string fsevent;

	private CharacterController _controller;

	public float MinSpeed = 2f;
    public float FrequencyFactor;
	private float _timeBeforeNextStep = 0f;
	private bool _stopped = true;

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
	}

	void Update()
    {
		var speed = _controller.velocity.magnitude;
		if (speed < MinSpeed)
		{
			_timeBeforeNextStep = 0.1F / (MinSpeed * FrequencyFactor);
		}
		else
		{
			if (_timeBeforeNextStep <= 0f)
			{
				FMODUnity.RuntimeManager.PlayOneShot(fsevent);

				_timeBeforeNextStep = 1 / (speed*FrequencyFactor);
			}
			_timeBeforeNextStep -= Time.deltaTime;
		}
		
	}
}
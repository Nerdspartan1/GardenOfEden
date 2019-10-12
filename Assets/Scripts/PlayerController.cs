﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public GameObject Monster;

	public float cameraSensitivityX = 100f;
	public float cameraSensitivityY = 100f;

	public float MoveSpeed = 10f;

	private float rotationY = 0F;

	private Camera _camera;
	private CharacterController _controller;
	private GlitchEffect _glitchEffect;

	private void Awake()
	{
		_camera = GetComponentInChildren<Camera>();
		_glitchEffect = GetComponentInChildren<GlitchEffect>();
		_controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		UpdateCameraRotation();
		UpdateMovement();
		UpdateGlitchEffect();
	}

	private void UpdateCameraRotation()
	{
		Cursor.lockState = CursorLockMode.Locked;
		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * cameraSensitivityX * Time.deltaTime;

		rotationY += Input.GetAxis("Mouse Y") * cameraSensitivityY * Time.deltaTime;
		rotationY = Mathf.Clamp(rotationY, -70f, +70f);

		transform.localEulerAngles = new Vector3(0, rotationX, 0);
		_camera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
	}

	private void UpdateMovement()
	{
		float forwardInput = Input.GetAxis("Vertical");
		float lateralInput = Input.GetAxis("Horizontal");

		Vector3 movement = transform.forward * forwardInput + transform.right * lateralInput;
		// cap the max speed so that the player doesn't go faster diagonally
		if (movement.sqrMagnitude > 1f) movement.Normalize(); 

		_controller.Move(movement * MoveSpeed * Time.deltaTime);
	}

	private void UpdateGlitchEffect()
	{
		float sqrDistanceToMonster = (Monster.transform.position - transform.position).sqrMagnitude;
		float intensityFactor = 10f*10f / sqrDistanceToMonster; //max intensity at 10 meter

		_glitchEffect.intensity = Mathf.Min(1f, intensityFactor);
		_glitchEffect.flipIntensity = Mathf.Min(1f, intensityFactor);
		float colorFactor = Mathf.Min(1f, intensityFactor);
		_glitchEffect.colorIntensity = colorFactor > 0.2f ? colorFactor : 0f; //add dead zone here bc even low factor changes color significantly
	}
}

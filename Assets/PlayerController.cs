using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float cameraSensitivityX = 100f;
	public float cameraSensitivityY = 100f;

	public float MoveSpeed = 10f;

	private float rotationY = 0F;

	private Camera _camera;
	private CharacterController _controller;

	private void Start()
	{
		_camera = GetComponentInChildren<Camera>();
		_controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		UpdateCameraRotation();
		UpdateMovement();
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
}

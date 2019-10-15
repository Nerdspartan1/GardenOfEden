using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
	public GameObject Monster;

	public float cameraSensitivityX = 100f;
	public float cameraSensitivityY = 100f;

	public float MoveSpeed = 10f;
	public float SprintSpeed = 11f;

	private float rotationY = 0F;

	private Camera _camera;
	private Grain _grain;
	private CharacterController _controller;
	private GlitchEffect _glitchEffect;
	private DeadPixelGenerator _deadPixelGenerator;

	private float _height;

	private void Awake()
	{
		_camera = GetComponentInChildren<Camera>();
		_grain = _camera.GetComponent<PostProcessVolume>().sharedProfile.GetSetting<Grain>();
		_glitchEffect = GetComponentInChildren<GlitchEffect>();
		_controller = GetComponent<CharacterController>();
		_deadPixelGenerator = GetComponentInChildren<DeadPixelGenerator>();

		_height = transform.position.y;
	}

	void Update()
	{
		UpdateCameraRotation();
		UpdateMovement();
		UpdateEffects();
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

		//keep us on the ground
		_controller.SimpleMove(1f * Vector3.down);
	}

	private void UpdateEffects()
	{
		float sqrDistanceToMonster = (Monster.transform.position - transform.position).sqrMagnitude;
		float intensityFactor = Mathf.Min(1f, 10f*10f / sqrDistanceToMonster); //max intensity at 10 meter

		_glitchEffect.intensity = intensityFactor;
		_glitchEffect.flipIntensity = intensityFactor;
		_glitchEffect.colorIntensity = intensityFactor > 0.2f ? intensityFactor : 0f; //add dead zone here bc even low factor changes color significantly
		_grain.intensity.value = intensityFactor;
		_deadPixelGenerator.Intensity = intensityFactor;

	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Collectible")
		{
			Destroy(other.gameObject);
			LevelManager.Instance.Collect();
		}
	}
}

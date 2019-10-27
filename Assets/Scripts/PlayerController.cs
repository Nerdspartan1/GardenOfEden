using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
	public Monster Monster;
	public GameObject Monolith;

	public float cameraSensitivity = 100f;

    public float MoveSpeed = 6f;

	private float rotationY = 0F;

	public bool Azerty = false;

	private float _confusedCameraAngle;
	private float _timeConfused = 0f;

    [HideInInspector]
	public Camera Camera;
	
	private ChromaticAberration _chromaticAberration;
	private CharacterController _controller;


	private float _height;

	private void Awake()
	{
		Camera = GetComponentInChildren<Camera>();
		
		_chromaticAberration = Camera.GetComponent<PostProcessVolume>().sharedProfile.GetSetting<ChromaticAberration>();
		_controller = GetComponent<CharacterController>();
		_height = transform.position.y;

	}

	void Update()
	{
		UpdateCameraRotation();
		UpdateMovement();
		UpdateEffects();

		if (Input.GetKeyDown(KeyCode.Escape))
			LevelManager.Instance.LeaveGame();

		if (_timeConfused > 0f) _timeConfused -= Time.deltaTime;
	}

	private void UpdateCameraRotation()
	{
		Cursor.lockState = CursorLockMode.Locked;
		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;

		rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
		rotationY = Mathf.Clamp(rotationY, -70f, +70f);

		transform.localEulerAngles = new Vector3(0, rotationX, 0);
		Camera.transform.localEulerAngles = new Vector3(-rotationY, 0, _timeConfused > 0f ? _confusedCameraAngle : 0f);
	}

	private void UpdateMovement()
	{
		float forwardInput = Azerty ? Input.GetAxis("VerticalAzerty") : Input.GetAxis("Vertical");
		float lateralInput = Azerty ? Input.GetAxis("HorizontalAzerty") : Input.GetAxis("Horizontal");

		Vector3 movement = transform.forward * forwardInput + transform.right * lateralInput;
		// cap the max speed so that the player doesn't go faster diagonally
		if (movement.sqrMagnitude > 1f) movement.Normalize();
		_controller.SimpleMove(movement * MoveSpeed);

	}

	private void UpdateEffects()
	{
		float sqrDistanceToMonolith = (transform.position - Monolith.transform.position).sqrMagnitude;
		float intensityFactor_monolith = Mathf.Min(1f, 3f*3f / sqrDistanceToMonolith); //max intensity at 3 meter
		_chromaticAberration.intensity.value = intensityFactor_monolith;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Collectible")
		{
			other.GetComponent<Collectible>().PickUp();
			LevelManager.Instance.Collect();
		}
	}

	public void Confuse(float duration)
	{
		_timeConfused = duration;
		_confusedCameraAngle = Random.Range(0, 360f);
	}

	public void SetAzerty(bool azerty)
	{
		Azerty = azerty;
		PlayerPrefs.SetInt("azerty", azerty ? 1 : 0);
	}

	public void SetSensitivity(float sensitivity)
	{
		cameraSensitivity = sensitivity;
		PlayerPrefs.SetFloat("sensitivity", sensitivity);
	}

}

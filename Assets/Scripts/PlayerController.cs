using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
	public Monster Monster;
	public GameObject Monolith;

	public float cameraSensitivityX = 100f;
	public float cameraSensitivityY = 100f;

    public float MoveSpeed = 6f;

	private float rotationY = 0F;

	public bool Azerty = false;



    [HideInInspector]
	public Camera Camera;
	private Grain _grain;
	private ChromaticAberration _chromaticAberration;
	private CharacterController _controller;
	//VFX
	private GlitchEffect _glitchEffect;
	private DeadPixelGenerator _deadPixelGenerator;
	private ApocalypseFilter _apocalypseFilter;

	private float _height;

	private void Awake()
	{
		Camera = GetComponentInChildren<Camera>();
		_grain = Camera.GetComponent<PostProcessVolume>().sharedProfile.GetSetting<Grain>();
		_chromaticAberration = Camera.GetComponent<PostProcessVolume>().sharedProfile.GetSetting<ChromaticAberration>();
		_glitchEffect = GetComponentInChildren<GlitchEffect>();
		_controller = GetComponent<CharacterController>();
		_deadPixelGenerator = GetComponentInChildren<DeadPixelGenerator>();
		_apocalypseFilter = GetComponentInChildren<ApocalypseFilter>();

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
		Camera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
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
		float distanceToMonster = Monster.PlayerDistanceInSight;
		float intensityFactor = Mathf.Min(1f, 10f / distanceToMonster); //max intensity at 10 meter

		float minIntensity = 0.1f * Mathf.Max(0, Monster.Aggressivity - 3);
		intensityFactor = Mathf.Max(minIntensity, intensityFactor);
		_glitchEffect.intensity = intensityFactor;
		_glitchEffect.flipIntensity = intensityFactor;
		_glitchEffect.colorIntensity = intensityFactor > 0.2f ? intensityFactor : 0f; //add dead zone here bc even low factor changes color significantly
		_grain.intensity.value = intensityFactor;
		_deadPixelGenerator.Intensity = intensityFactor;

		_apocalypseFilter.enabled = (Monster.Aggressivity >= 5) && intensityFactor > 0.5f;

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

	public void SetAzerty(bool azerty)
	{
		Azerty = azerty;
		PlayerPrefs.SetInt("azerty", azerty ? 1 : 0);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Sprint : MonoBehaviour
{
	private PlayerController _playerController;

    public float WalkSpeed = 3f;
	public float SprintSpeed = 5.5f;

	[Range(0,1f)]
	public float Stamina = 1f;
	public float StaminaDepletionRate = 0.1f;
	public float StaminaRegenerationRate = 0.05f;
	public float StaminaSprintThreshold = 0.25f;

	private bool _isRunning = false;

	// Start is called before the first frame update
	void Start()
    {
		_playerController = GetComponent<PlayerController>();
		_playerController.MoveSpeed = WalkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetButtonDown("Fire3") && Stamina > StaminaSprintThreshold)
		{
			_isRunning = true;
			_playerController.MoveSpeed = SprintSpeed;
		}

		if(Input.GetButtonUp("Fire3") || Stamina <= 0f)
		{
			_isRunning = false;
			_playerController.MoveSpeed = WalkSpeed;
			if(Stamina < 0f) Stamina = 0f;
		}

		if (_isRunning)
		{
			Stamina -= StaminaDepletionRate * Time.deltaTime;
		}
		else
		{
			if(Stamina <= 1f)
				Stamina += StaminaRegenerationRate * Time.deltaTime;

			if (Stamina > 1f) Stamina = 1f;

		}
    }
}

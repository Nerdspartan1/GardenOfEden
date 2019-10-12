using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	private float rotationY = 0F;

	private Camera _camera;

	private void Start()
	{
		_camera = GetComponentInChildren<Camera>();
	}

	void Update()
	{
		Cursor.lockState = CursorLockMode.Locked;
		float rotationX = _camera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;

		rotationY += Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
		rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

		_camera.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
	}
}

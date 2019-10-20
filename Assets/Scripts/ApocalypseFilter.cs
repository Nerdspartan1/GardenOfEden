using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ApocalypseFilter : MonoBehaviour
{
	public CameraShader Shader;
	private Camera _camera;
	// Start is called before the first frame update
	void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	private void OnEnable()
	{
		_camera.clearFlags = CameraClearFlags.Nothing;
		Shader.enabled = true;
	}

	private void OnDisable()
	{
		_camera.clearFlags = CameraClearFlags.Color;
		Shader.enabled = false;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShader : MonoBehaviour
{
	public Shader Shader;
	private Material _material;

	void Start()
	{
		_material = new Material(Shader);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, _material);
	}
}

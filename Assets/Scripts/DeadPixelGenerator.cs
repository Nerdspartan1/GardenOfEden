using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadPixelGenerator : MonoBehaviour
{
	public Transform GameCanvas;
	public GameObject DeadPixel;
	[Range(0, 1)]
	public float Intensity = 0f;
	public int MaxDeadPixels = 100;
	public float SpawnRate = 1f;

	private float _timeToNextUpdate;
	private int _numberOfDeadPixels = 0;
	// Start is called before the first frame update
	void Start()
    {
		_timeToNextUpdate = 1f / SpawnRate;
    }

    // Update is called once per frame
    void Update()
    {
		if (_numberOfDeadPixels < MaxDeadPixels)
		{
			if (_timeToNextUpdate <= 0f)
			{
				if (Random.Range(0f, 1f) < Intensity * Intensity) AddDeadPixel();
				_timeToNextUpdate = 1f / SpawnRate;
			}
			_timeToNextUpdate -= Time.deltaTime;
		}
    }

	private void AddDeadPixel()
	{
		Vector2Int pos = new Vector2Int(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
		var deadPixel = Instantiate(DeadPixel, new Vector3(pos.x, pos.y, 0), Quaternion.identity, GameCanvas);
		var image = deadPixel.GetComponent<Image>();
		switch(Random.Range(0, 3))
		{
			case 0:
				image.color = Color.red;
				break;
			case 1:
				image.color = Color.green;
				break;
			case 2:
				image.color = Color.blue;
				break;
		}
		_numberOfDeadPixels++;
	}
}

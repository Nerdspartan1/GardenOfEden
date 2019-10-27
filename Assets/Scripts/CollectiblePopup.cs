using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class CollectiblePopup : MonoBehaviour
{
	private RawImage[] _rawImages;
	private Animator _animator;
	public float ShowUpTime = 4f;
	private float _timeBeforeHide = 0f;

	private int _numberCollected = 0;

	private void Start()
	{
		_animator = GetComponent<Animator>();
		_rawImages = GetComponentsInChildren<RawImage>();
	}

	public void Add(RenderTexture renderTexture)
	{
		if(_numberCollected < 6)
		{
			_rawImages[_numberCollected].enabled = true;
			_rawImages[_numberCollected++].texture = renderTexture;
			ShowUp();
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.Space))
			ShowUp();
		
		if(_timeBeforeHide > 0f)
		{
			_timeBeforeHide -= Time.deltaTime;
			if (_timeBeforeHide <= 0f) Hide();
		}
    }

	public void ShowUp()
	{
		_animator.SetBool("show", true);
		_timeBeforeHide = ShowUpTime;
	}

	public void Hide()
	{
		_animator.SetBool("show", false);
		_timeBeforeHide = 0f;
	}
}

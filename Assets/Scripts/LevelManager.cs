using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(TileGenerator))]
public class LevelManager : MonoBehaviour
{

	public static LevelManager Instance;
	public Monster Monster;
	public int NumberOfCollectibles;
	[SerializeField]
	private int _numberOfCollectiblesLeft;

    //Adding fmod entity sound + chase audio

    FMOD.Studio.EventInstance SoundscapeEvent;

    private void Awake()
	{
		Instance = this;
		
	}

	void Start()
    {
		_numberOfCollectiblesLeft = NumberOfCollectibles;

        SoundscapeEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Soundscape/BGM");
        SoundscapeEvent.start();
    }

	//DEBUG
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			Collect();
		}
	}

	public void Collect()
	{
		if (_numberOfCollectiblesLeft > 0)
		{
            SoundscapeEvent.setParameterByName("Items Collected", 1f);

			_numberOfCollectiblesLeft--;
			Monster.LevelUpAggressivity(NumberOfCollectibles - _numberOfCollectiblesLeft);
		}
	}


	public void WriteMessage()
	{
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
		StreamWriter writer = new StreamWriter(path + "\\dontcomeback.txt");
		char[] chars = new char[666];
		for (int i = 0; i < 666; ++i) chars[i] = (char)Random.Range(0, 256);
		writer.WriteLine(new string(chars) + "There is nothing for you here" + new string(chars));
		writer.Close();
	}

}

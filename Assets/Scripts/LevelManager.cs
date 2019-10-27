using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(PropsManager))]
public class LevelManager : MonoBehaviour
{

	public static LevelManager Instance;
	public Monster Monster;
	private PropsManager _propsManager;
	private Ending _ending;

	private int _numberOfCollectiblesLeft;

	public Color StartFogColor;
	public Color EndFogColor;

	public FMOD.Studio.Bus EverythingbutMenuBus;

	//Adding fmod item progression and soundscape

	public FMOD.Studio.EventInstance SoundscapeEvent;

    private void Awake()
	{
		Instance = this;
		_propsManager = GetComponent<PropsManager>();
		_ending = GetComponent<Ending>();

		EverythingbutMenuBus = FMODUnity.RuntimeManager.GetBus("Bus:/Everything but Menu");
	}

	void Start()
    {
		_numberOfCollectiblesLeft = _propsManager.Collectibles.Length;

        SoundscapeEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Soundscape/BGM");
        SoundscapeEvent.start();
    }

	
	private void Update()
	{
		//DEBUG
		//if (Input.GetKeyDown(KeyCode.P))
		//{
		//	Collect();
		//}

	}

	public void Collect()
	{
		if (_numberOfCollectiblesLeft > 0)
		{
			_numberOfCollectiblesLeft--;
			Monster.LevelUpAggressivity(_propsManager.Collectibles.Length - _numberOfCollectiblesLeft);

            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Item Pickup");
            SoundscapeEvent.setParameterByName("Items Collected", (float)Monster.Aggressivity);
            
            if (_numberOfCollectiblesLeft == 0)
			{
				_ending.enabled = true;
			}
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

	public void LeaveGame(bool audioFade = false)
	{
		FMOD.Studio.PARAMETER_DESCRIPTION pd;
		FMOD.Studio.PARAMETER_ID EnemyDistID;

		FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("Enemy Dist", out pd);
		EnemyDistID = pd.id;

		FMODUnity.RuntimeManager.StudioSystem.setParameterByID(EnemyDistID, 0f);

		LevelManager.Instance.SoundscapeEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		LevelManager.Instance.SoundscapeEvent.release();

		GameManager.Instance.RestartGame();

		if(!audioFade) EverythingbutMenuBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}

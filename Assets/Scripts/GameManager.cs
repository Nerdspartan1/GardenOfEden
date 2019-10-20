﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameObject Game;
	public GameObject Menu;

    FMOD.Studio.Bus MasterBus;
    FMOD.Studio.EventInstance MenuMusicEvent;

    void Awake()
    {
		Instance = this;
		Game.SetActive(false);
		Menu.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

	}

    void Start()
    {
        MenuMusicEvent = FMODUnity.RuntimeManager.CreateInstance("event:/BGM/Menu");
        MenuMusicEvent.start();
    }

	public void StartGame()
	{
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Button");
		Menu.SetActive(false);
		Game.SetActive(true);
        //MenuMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Menu", 0f);
        MenuMusicEvent.release();
		Cursor.visible = false;
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Menu", 1f);
        MenuMusicEvent.start();
    }

	public void Quit()
	{
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Button");
        Application.Quit();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameObject Game;
	public GameObject Menu;

    FMOD.Studio.Bus MasterBus;

    void Awake()
    {
		Instance = this;
		Game.SetActive(false);
		Menu.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
    }

	public void StartGame()
	{
		Menu.SetActive(false);
		Game.SetActive(true);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void Quit()
	{
		Application.Quit();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject Game;
	public GameObject Menu;

    void Awake()
    {
		Game.SetActive(false);
		Menu.SetActive(true);
    }

	public void StartGame()
	{
		Menu.SetActive(false);
		Game.SetActive(true);
	}

	public void Quit()
	{
		Application.Quit();
	}

}

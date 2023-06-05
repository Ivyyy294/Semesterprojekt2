using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	public void OnNewGameButton()
	{
		BlackBoard.Me().Clear();
		SceneManager.LoadScene (1);
	}

	public void OnContinueButton()
	{
		BlackBoard.Me().Clear();
		SceneManager.LoadScene (1);
	}

    public void OnExitButton()
	{
		Application.Quit();
	}
}

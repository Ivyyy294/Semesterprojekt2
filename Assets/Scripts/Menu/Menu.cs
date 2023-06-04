using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	[SerializeField] UnityEditor.SceneAsset gameScene;

	public void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	public void OnNewGameButton()
	{
		SceneManager.LoadScene (gameScene.name);
	}

	public void OnContinueButton()
	{
		SceneManager.LoadScene (gameScene.name);
	}

    public void OnExitButton()
	{
		Application.Quit();
	}
}

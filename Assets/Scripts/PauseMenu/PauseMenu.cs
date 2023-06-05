using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] GameEvent closeEvent;

	public void OnContinue()
	{
		closeEvent?.Raise();
	}

	public void OnMenu()
	{
		SceneManager.LoadScene (0);
	}

	public void OnExit()
	{
		Application.Quit();
	}

	private void OnEnable()
	{
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Confined;
	}

	private void OnDisable()
	{
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Locked;
	}
}

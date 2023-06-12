using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] GameEvent closeEvent;
	[SerializeField] GameEvent loadMenu;
	[SerializeField] GameEvent closeGame;

	public void OnContinue()
	{
		closeEvent?.Raise();
	}

	public void OnMenu()
	{
		loadMenu?.Raise();
	}

	public void OnExit()
	{
		closeGame?.Raise();
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

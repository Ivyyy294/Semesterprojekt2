using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class PauseMenu : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] GameEvent showSettings;
	[SerializeField] GameEvent closeEvent;
	[SerializeField] GameEvent loadMenu;
	[SerializeField] GameEvent closeGame;

	public void OnSettings()
	{
		showSettings?.Raise();
	}

	public void OnContinue()
	{
		closeEvent?.Raise();
	}

	public void OnMenu()
	{
		SaveGameManager.Me().SaveGameState();
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

	private void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			OnContinue();
	}
}

using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class PauseMenu : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] GameObject settings;
	[SerializeField] GameEvent closeEvent;
	[SerializeField] GameEvent loadMenu;
	[SerializeField] GameEvent closeGame;

	public void OnSettings()
	{
		settings.SetActive(true);
	}

	public void OnContinue()
	{
		settings.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;
		Time.timeScale = 1f;
		gameObject.SetActive(false);
	}

	public void OnMenu()
	{
		Time.timeScale = 1f;
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

	private void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			OnContinue();
	}
}

using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] GameEvent closeEvent;

	public void OnContinue()
	{
		closeEvent?.Raise();
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

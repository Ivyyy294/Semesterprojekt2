using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckTerminal : MonoBehaviour
{
	[SerializeField] Ivyyy.GameEvent.GameEvent closeEvent;
	[SerializeField] Ivyyy.GameEvent.GameEvent settingsEvent;

	public void OnSettings()
	{
		settingsEvent?.Raise();
	}

	public void OnClose()
	{
		closeEvent.Raise();
	}

	//Private Functions
	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}
	private void FixedUpdate()
	{
		Canvas.ForceUpdateCanvases();
	}
}

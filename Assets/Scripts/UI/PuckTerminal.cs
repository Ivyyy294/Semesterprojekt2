using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckTerminal : MonoBehaviour
{
	[SerializeField] Ivyyy.GameEvent.GameEvent closeEvent;

	//Public Funtions
	//public void Show(bool val)
	//{
	//	gameObject.SetActive (val);
	//}

	public void RaiseCloseEvent()
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
}

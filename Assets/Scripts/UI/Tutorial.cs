using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	[SerializeField] GameEvent skipEvent;
	[SerializeField] GameEvent backEvent;

    public void OnSkip()
	{
		Cursor.lockState = CursorLockMode.Locked;
		skipEvent?.Raise();
	}

	public void OnBack()
	{
		backEvent?.Raise();
	}
}

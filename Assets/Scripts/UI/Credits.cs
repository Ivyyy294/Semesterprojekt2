using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
	[SerializeField] GameEvent menuEvent;

	public void ShowMenu()
	{
		menuEvent?.Raise();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			ShowMenu();
	}
}

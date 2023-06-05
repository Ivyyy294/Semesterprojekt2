using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class ChatTerminalObj : MonoBehaviour, Ivyyy.Interfaces.InteractableObject
{
	[SerializeField] GameEvent gameEvent;

	public void Interact()
	{
		gameEvent?.Raise();
	}
}

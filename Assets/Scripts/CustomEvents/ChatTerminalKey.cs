using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class ChatTerminalKey : MonoBehaviour, InteractableObject
{
	[SerializeField] PuckTerminal chatTerminal;
	[SerializeField] AudioAsset audioAsset;

	public void Interact()
	{
		chatTerminal.SetPasswordAvailable (true);
		audioAsset?.PlayAtPos(transform.position);
		gameObject.SetActive(false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class ChatTerminalKey : MonoBehaviour, InteractableObject
{
	[SerializeField] ChatTerminalObj chatTerminalObj;
	[SerializeField] AudioAsset audioAsset;

	public void Interact()
	{
		chatTerminalObj.SetLocked(false);
		audioAsset?.PlayAtPos(transform.position);
		gameObject.SetActive(false);
	}
}

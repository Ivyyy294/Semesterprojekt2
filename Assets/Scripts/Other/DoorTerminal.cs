using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
public class DoorTerminal : MonoBehaviour, InteractableObject
{
	[SerializeField] AudioAsset audioAsset;
	[SerializeField] float coolDown = 0.5f;
	float timer = 0f;

    public void Interact()
	{
		if (timer >= coolDown)
		{
			audioAsset?.Play();
			timer = 0f;
		}
	}

	private void Update()
	{
		if (timer < coolDown)
			timer += Time.deltaTime;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
public class DoorTerminal : AudioPlayer, InteractableObject
{
	[SerializeField] float coolDown = 0.5f;
	float timer = 0f;

    public void Interact()
	{
		if (timer >= coolDown)
		{
			PlayAtPos(transform.position);
			timer = 0f;
		}
	}

	private void Update()
	{
		if (timer < coolDown)
			timer += Time.deltaTime;
	}
}

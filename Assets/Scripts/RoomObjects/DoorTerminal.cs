using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
public class DoorTerminal : AudioPlayer, InteractableObject
{
	public bool active;
	[SerializeField] float coolDown = 0.5f;
	float timer = 0f;
	public bool locked = true;

    public void Interact()
	{
		if (locked && timer >= coolDown)
		{
			active = false;
			Play();
			timer = 0f;
		}
		else if (!locked)
		{
			active = true;
		}
	}

	private void Update()
	{
		if (timer < coolDown)
			timer += Time.deltaTime;
	}
}

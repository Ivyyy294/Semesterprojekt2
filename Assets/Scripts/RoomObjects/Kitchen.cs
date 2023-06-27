using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class Kitchen : MonoBehaviour, InteractableObject
{
	bool active = false;
	Animator animator;

	public bool IsActive() {return active;}

	public void Interact()
	{
		active = !active;

		if (active)
			animator?.SetTrigger ("Open");
		else
			animator?.SetTrigger ("Close");
	}

	private void Start()
	{
		animator = GetComponent <Animator>();
	}
}

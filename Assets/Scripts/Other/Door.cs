using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class Door : MonoBehaviour, InteractableObject
{
	[SerializeField] Animator animator;
	public bool active = false;

    public void Interact()
	{
		active = !active;
	}

	void Update()
	{		
		animator.SetBool ("open", active);
	}
}

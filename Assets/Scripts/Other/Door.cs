using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class Door : MonoBehaviour, InteractableObject
{
	[SerializeField] Animator animator;
	[SerializeField] AudioAsset audioAsset;
	public bool active = false;

    public void Interact()
	{
		active = !active;

		if(active)
			audioAsset?.Play();
	}

	void Update()
	{		
		animator.SetBool ("open", active);
	}
}

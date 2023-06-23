using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class Door : MonoBehaviour, InteractableObject
{
	[SerializeField] AudioAsset audioAsset;
	public Animator animator;
	bool active = false;
	bool glitch = false;

    public void Interact()
	{
		active = !active;
	}

	public void SetGlitch (bool val)
	{
		glitch = val;
	}

	void Update()
	{		
		animator.SetBool ("glitch", glitch);
		animator.SetBool ("open", active);
	}
}

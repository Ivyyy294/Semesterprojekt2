using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class CryoDoor : MonoBehaviour, InteractableObject
{
	Animator animator;
	bool open = false;
	public void Interact()
	{
		open = !open;

		if (open)
			animator?.SetTrigger("Open");
		else
			animator?.SetTrigger ("Close");
	}

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent <Animator>();

		if (animator == null)
			Debug.LogError ("Missing Animator!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

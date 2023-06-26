using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;

public class Player : MonoBehaviour
{
	private static Player me;

	private CharacterController characterController;
	private MouseLook mouseLook;
	private PlayerInteraction playerInteraction;

	public static Player Me()
	{
		return me;
	}

	public void Lock ()
	{
		characterController.enabled = false;
		mouseLook.enabled = false;
		playerInteraction.enabled = false;
	}

	public void Unlock ()
	{
		characterController.enabled = true;
		mouseLook.enabled = true;
		playerInteraction.enabled = true;
	}

	public void BlockInteractions (bool val)
	{
		if (val)
			playerInteraction.tagFilter = "AllwaysInteractable";
		else
			playerInteraction.tagFilter = null;
	}

    // Start is called before the first frame update
    void Start()
    {
        if (me== null)
		{
			me = this;

			characterController = GetComponent <CharacterController>();
			mouseLook = GetComponent <MouseLook>();
			playerInteraction = GetComponent <PlayerInteraction>();
		}
		else
			Destroy(this);
    }
}

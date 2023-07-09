using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;
using Ivyyy.Core;

public class Player : MonoBehaviour
{
	//Public values
	[HideInInspector] public MouseLook mouseLook;
	[HideInInspector] public Cinemachine.CinemachineBrain cinemachineBrain;
	public InteractTextOverlay interactTextOverlay;
	
	//Private values	
	private static Player me;

	private PlayerMovement3D playerMovement3D;
	private CharacterController characterController;
	private PlayerInteraction playerInteraction;
	private PlayerZoom playerZoom;
	bool locked = false;

	public static Player Me()
	{
		if (me == null)
			me = Camera.main.transform.parent.GetComponent <Player>();

		return me;
	}

	public float GetRange() {return playerInteraction.GetRange();}

	public bool IsLocked() { return locked;}

	public void Lock ()
	{
		playerMovement3D.enabled = false;
		mouseLook.enabled = false;
		playerInteraction.enabled = false;
		playerZoom.enabled = false;
		locked = true;
	}

	public void Unlock ()
	{
		playerMovement3D.enabled = true;
		mouseLook.enabled = true;
		playerInteraction.enabled = true;
		playerZoom.enabled = true;
		locked = false;
	}

	public void BlockInteractions (bool val, string filter)
	{
		if (val)
			playerInteraction.tagFilter = filter;
		else
			playerInteraction.tagFilter = null;
	}

    // Start is called before the first frame update
    void Awake()
    {
        if (me== null)
		{
			me = this;

			characterController = GetComponent <CharacterController>();
			mouseLook = GetComponent <MouseLook>();
			playerInteraction = GetComponent <PlayerInteraction>();
			playerMovement3D = GetComponent <PlayerMovement3D>();
			cinemachineBrain = Camera.main.GetComponent <Cinemachine.CinemachineBrain>();
			playerZoom = GetComponent <PlayerZoom>();
		}
		else
			Destroy(this);
    }
}

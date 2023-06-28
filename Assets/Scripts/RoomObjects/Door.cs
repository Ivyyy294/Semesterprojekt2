using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
using Ivyyy.StateMachine;


public class Door : PushdownAutomata, InteractableObject
{
	public class OpenState : IState
	{
		public bool open;

		public void Enter (GameObject obj)
		{
			Door door = obj.GetComponent <Door>();
			door?.animator?.SetBool ("open", true);
		}

		public void Update (GameObject obj)
		{
			Door door = obj.GetComponent <Door>();
			
			if (door != null)
			{
				if (door.glitch)
					door.PushState (door.glitchState);
				else if (!door.open)
					door?.PopState();
			}
		}

		public void Exit(GameObject obj)
		{
			Door door = obj.GetComponent <Door>();
			door?.animator?.SetBool ("open", false);
		}
	}

	public class ClosedState : IState
	{
		public void Enter (GameObject obj) {}

		public void Update (GameObject obj)
		{
			Door door = obj.GetComponent <Door>();

			if (door != null)
			{
				if (door.glitch)
					door.PushState (door.glitchState);
				else if (door.open)
					door.PushState (door.openState);
			}
		}

		public void Exit(GameObject obj) {}
	}

	public class GlitchState : IState
	{
		public void Enter (GameObject obj)
		{
			Door door = obj.GetComponent <Door>();
			door?.animator?.SetBool ("glitch", true);
		}

		public void Update (GameObject obj)
		{
			Door door = obj.GetComponent <Door>();
			
			if (door != null && !door.glitch)
			{
				door.PopState();
			}
		}

		public void Exit(GameObject obj)
		{
			Door door = obj.GetComponent <Door>();
			door?.animator?.SetBool ("glitch", false);
		}
	}

	public OpenState openState = new OpenState();
	public ClosedState closedState = new ClosedState();
	public GlitchState glitchState = new GlitchState();

	[SerializeField] AudioAsset audioAsset;
	public Animator animator;
	public bool open = false;
	public bool glitch = false;

	private void Start()
	{
		PushState (closedState);
	}

	public void Interact()
	{
		open = !open;
	}

	public void SetGlitch (bool val)
	{
		glitch = val;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
using Ivyyy.GameEvent;
using Ivyyy.StateMachine;
using Cinemachine;

public class InteractableCamera : FiniteStateMachine, InteractableObject
{
	[System.Serializable]
	public abstract class BaseState : IState
	{
		protected InteractableCamera interactableCamera;

		public virtual void Enter (GameObject obj)
		{
			interactableCamera = obj.GetComponent <InteractableCamera>();
		}

		public abstract void Update (GameObject obj);

		public virtual void Exit(GameObject obj) {}
	}

	public class InactiveState : BaseState
	{
		public bool activate;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			activate = false;
		}

		public override void Update (GameObject obj)
		{
			if (activate)
				interactableCamera.EnterState (interactableCamera.easeInState);
		}
	}

	[System.Serializable]
	public class EaseInState : BaseState
	{
		CinemachineBrain cinemachineBrain;
		public override void Enter (GameObject obj)
		{
			base.Enter(obj);
			Player.Me().Lock();
			interactableCamera?.cameraContainer.SetActive(true);
			cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
		}

		public override void Update (GameObject obj)
		{
			if (cinemachineBrain != null && !cinemachineBrain.IsBlending)
				interactableCamera.EnterState (interactableCamera.activeState);
		}
	}

	[System.Serializable]
	public class ActiveState : BaseState
	{
		[SerializeField] MouseLook mouseLook;
		bool locked = false;

		public void SetLocked (bool val) {locked = val;}

		public override void Enter (GameObject obj)
		{
			base.Enter(obj);

			if (mouseLook)
			{
				mouseLook.ResetRotation();
				mouseLook.enabled = true;
			}
		}

		public override void Update (GameObject obj)
		{
			if (!locked && Input.GetKeyDown(KeyCode.F))
			{
				mouseLook.enabled = false;
				interactableCamera.EnterState (interactableCamera.easeOutState);
			}
		}
	}

	[System.Serializable]
	public class EaseOutState : BaseState
	{
		public bool applyForwardToPlayer = true;
		CinemachineBrain cinemachineBrain;
		public override void Enter (GameObject obj)
		{
			base.Enter(obj);
			
			if (applyForwardToPlayer)
			{
				Player.Me().mouseLook.ResetRotation();
				Player.Me().transform.forward = interactableCamera.cameraContainer.transform.forward;
			}

			interactableCamera?.cameraContainer.SetActive(false);
			cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
		}

		public override void Update (GameObject obj)
		{
			if (cinemachineBrain != null && !cinemachineBrain.IsBlending)
			{
				Player.Me().Unlock();
				interactableCamera.EnterState (interactableCamera.inactiveState);
			}
		}

		public override void Exit(GameObject obj)
		{
			if (interactableCamera != null && interactableCamera.cameraContainer != null)
			{
				interactableCamera.cameraContainer.transform.rotation = interactableCamera.defaultRotation;
				interactableCamera.cameraContainer.GetComponentInChildren <CinemachineVirtualCamera>().transform.localRotation = Quaternion.identity;
			}
		}
	}

	[Header ("Lara Values")]
	public InactiveState inactiveState = new InactiveState();
	public EaseInState easeInState = new EaseInState();
	public ActiveState activeState = new ActiveState();
	public EaseOutState easeOutState = new EaseOutState();
	public GameObject cameraContainer;
	public Quaternion defaultRotation;

	public bool IsActive () {return currentState == activeState;}

	public void Interact()
	{
		inactiveState.activate = true;
		
		if (cameraContainer!= null)
			defaultRotation = cameraContainer.transform.rotation;
	}

	//Private
	private void Start()
	{
		EnterState (inactiveState);
	}
}

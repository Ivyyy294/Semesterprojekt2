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
		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			interactableCamera.active = false;
		}

		public override void Update (GameObject obj)
		{
			if (interactableCamera.active)
				interactableCamera.EnterState (interactableCamera.easeInState);
		}
	}

	[System.Serializable]
	public class EaseInState : BaseState
	{
		[SerializeField] GameEvent lockPlayer;
		CinemachineBrain cinemachineBrain;
		public override void Enter (GameObject obj)
		{
			base.Enter(obj);
			lockPlayer?.Raise();
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
		public override void Enter (GameObject obj)
		{
			base.Enter(obj);

			if (mouseLook)
				mouseLook.enabled = true;
		}

		public override void Update (GameObject obj)
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				mouseLook.enabled = false;
				interactableCamera.EnterState (interactableCamera.easeOutState);
			}
		}
	}

	[System.Serializable]
	public class EaseOutState : BaseState
	{
		[SerializeField] GameEvent releasePlayer;
		CinemachineBrain cinemachineBrain;
		public override void Enter (GameObject obj)
		{
			base.Enter(obj);
			interactableCamera?.cameraContainer.SetActive(false);
			cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
		}

		public override void Update (GameObject obj)
		{
			if (cinemachineBrain != null && !cinemachineBrain.IsBlending)
			{
				releasePlayer?.Raise();
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
	public bool active;
	public Quaternion defaultRotation;

	public void Interact()
	{
		if (!active)
			active = true;
		
		if (cameraContainer!= null)
			defaultRotation = cameraContainer.transform.rotation;
	}

	//Private
	private void Start()
	{
		EnterState (inactiveState);
	}
}

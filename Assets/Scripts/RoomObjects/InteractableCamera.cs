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
			interactableCamera?.cameraContainer.gameObject.SetActive(true);
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
		bool locked = false;

		public void SetLocked (bool val) {locked = val;}

		public override void Enter (GameObject obj)
		{
			base.Enter(obj);
			Player.Me().interactTextOverlay.Show (true);

			if (interactableCamera.cameraContainer != null)
				interactableCamera.cameraContainer.enabled = true;
		}

		public override void Update (GameObject obj)
		{
			if (!locked && Input.GetKeyDown(KeyCode.F))
			{
				interactableCamera.cameraContainer.enabled = false;
				interactableCamera.EnterState (interactableCamera.easeOutState);
			}
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().interactTextOverlay.Show (false);
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
			
			interactableCamera?.cameraContainer.gameObject.SetActive(false);
			cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

			if (applyForwardToPlayer)
				Player.Me().transform.forward = interactableCamera.cameraContainer.transform.forward;
		}

		public override void Update (GameObject obj)
		{
			if (cinemachineBrain != null && !cinemachineBrain.IsBlending)
			{
				Player.Me().Unlock();
				interactableCamera.EnterState (interactableCamera.inactiveState);
			}
		}
	}

	[System.Serializable]
	public class SpawnState : BaseState
	{
		public float playerSpawnRotationX = 0f;
		CinemachineBrain cinemachineBrain;
		CinemachineBlendDefinition.Style defaultBlendStyle;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			cinemachineBrain = Player.Me().cinemachineBrain;
			defaultBlendStyle = cinemachineBrain.m_DefaultBlend.m_Style;
			cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

			Player.Me().Lock();
			Player.Me().mouseLook.SetRotationX (playerSpawnRotationX);

			interactableCamera?.cameraContainer.gameObject.SetActive(true);
		}

		public override void Update (GameObject obj)
		{
			cinemachineBrain.m_DefaultBlend.m_Style = defaultBlendStyle;
			interactableCamera.EnterState (interactableCamera.activeState);
		}
	}

	[Header ("Lara Values")]
	public InactiveState inactiveState = new InactiveState();
	public EaseInState easeInState = new EaseInState();
	public ActiveState activeState = new ActiveState();
	public EaseOutState easeOutState = new EaseOutState();
	public SpawnState spawnState = new SpawnState();
	public MouseLook cameraContainer;
	public GameObject vCam;
	public Quaternion defaultRotationContainer;
	public Quaternion defaultRotationCam;

	public bool IsActive () {return currentState == activeState;}

	public void Interact()
	{
		cameraContainer.transform.rotation = defaultRotationContainer;
		vCam.transform.localRotation = defaultRotationCam;
		inactiveState.activate = true;
	}

	//Private
	private void Start()
	{
		defaultRotationContainer = cameraContainer.transform.rotation;
		defaultRotationCam = vCam.transform.localRotation;
		EnterState (inactiveState);
	}
}

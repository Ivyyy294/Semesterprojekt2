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

	[System.Serializable]
	public class InactiveState : BaseState
	{
		[SerializeField] AudioAsset audioAsset;
		public bool activate;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			activate = false;
		}

		public override void Update (GameObject obj)
		{
			if (activate)
			{
				audioAsset?.PlayAtPos(interactableCamera.transform.position);
				interactableCamera.EnterState (interactableCamera.easeInState);
			}
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
		[SerializeField] bool allowInteractions;
		bool interactionActive;
		bool locked = false;

		public void SetLocked (bool val) {locked = val;}

		public override void Enter (GameObject obj)
		{
			base.Enter(obj);

			if (interactableCamera.cameraContainer != null)
				interactableCamera.cameraContainer.enabled = true;

			interactionActive = false;
		}

		public override void Update (GameObject obj)
		{
			Player.Me().interactTextOverlay.Show (!locked);

			if (!locked && Input.GetKeyDown(KeyCode.F))
			{
				if (allowInteractions)
				{
					if (interactionActive)
					{
						interactionActive = false;
						return;
					}
					else if (InteractableObjectInSight())
					{
						interactionActive = true;
						return;
					}
				}

				interactableCamera.cameraContainer.enabled = false;
				interactableCamera.EnterState (interactableCamera.easeOutState);
			}
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().interactTextOverlay.Show (false);
		}

		bool InteractableObjectInSight ()
		{
			InteractableObject interactableObject;
			Ray ray = new Ray (interactableCamera.cameraContainer.cameraTrans.transform.position, interactableCamera.cameraContainer.cameraTrans.transform.forward);
			//Ignore Player layer
			int layerMask = 1 << 0;
			RaycastHit hit;

			bool inRange = false;
			float range = Player.Me().GetRange();

			if (Physics.Raycast (ray, out hit, range, layerMask))
			{
				interactableObject = hit.transform.gameObject.GetComponent<InteractableObject>();
				MonoBehaviour behaviour = (MonoBehaviour)interactableObject;

				if (interactableObject != null && behaviour.enabled)
				{
					inRange = true;
					interactableObject.Interact();
				}
			}

			Debug.DrawRay (ray.origin, ray.direction * range, inRange ? Color.green : Color.red);

			return inRange;
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
				interactableCamera.EnterState (interactableCamera.inactiveState);
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().Unlock();
		}
	}

	[System.Serializable]
	public class SpawnState : BaseState
	{
		public float playerSpawnRotationX = 0f;
		CinemachineBrain cinemachineBrain;
		CinemachineBlendDefinition.Style defaultBlendStyle;
		float timer;

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
			if (timer < 0.5f)
				timer += Time.deltaTime;
			else if (!cinemachineBrain.IsBlending)
			{
				cinemachineBrain.m_DefaultBlend.m_Style = defaultBlendStyle;
				interactableCamera.EnterState (interactableCamera.activeState);
			}
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

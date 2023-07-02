using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;
using Cinemachine;
using Ivyyy.StateMachine;

public abstract class BaseState : IState
{
	protected ChatTerminalObj terminalObj;

	public virtual void Enter (GameObject obj)
	{
		terminalObj = obj.GetComponent<ChatTerminalObj>();
	}

	public abstract void Update (GameObject obj);

	public virtual void Exit(GameObject obj){}
}

public class OfflineState : BaseState
{
	public override void Enter (GameObject obj)
	{
		base.Enter (obj);
		terminalObj.terminalCamera.Priority = 0;
	}

	public override void Update (GameObject obj)
	{
		if (terminalObj.locked)
			terminalObj.EnterState (terminalObj.lockedState);
		else if (terminalObj.active)
			terminalObj.EnterState (ChatTerminalObj.powerUpState);
	}
}

public class PowerUpState : BaseState
{
	float timer = 0f;

	public override void Enter (GameObject obj)
	{
		base.Enter (obj);
		Player.Me().Lock();
		terminalObj.terminalCamera.Priority = 2;
		timer = 0f;
	}

	public override void Update (GameObject obj)
	{
		if (timer >= terminalObj.minTransitionTime && !terminalObj.cinemachineBrain.IsBlending)
			terminalObj.EnterState (ChatTerminalObj.onlineState);
		else
			timer += Time.deltaTime;
	}
}

public class PowerDownState : BaseState
{
	float timer = 0f;

	public override void Enter (GameObject obj)
	{
		base.Enter(obj);
		Player.Me().mouseLook.SetRotationX (0f);
		terminalObj.terminalCamera.Priority = 0;
		timer = 0f;
	}

	public override void Update (GameObject obj)
	{
		if (timer >= terminalObj.minTransitionTime && !terminalObj.cinemachineBrain.IsBlending)
			terminalObj.EnterState (ChatTerminalObj.offlineState);
		else
			timer += Time.deltaTime;
	}

	public override void Exit(GameObject obj)
	{
		Player.Me().Unlock();
	}
}

public class OnlineState : BaseState
{
	public override void Enter (GameObject obj)
	{
		base.Enter(obj);
		terminalObj.terminalUiShow?.Raise();
	}

	public override void Update (GameObject obj)
	{
		if (!terminalObj.active)
			terminalObj.EnterState (ChatTerminalObj.powerDownState);
	}
}

[System.Serializable]
public class LockedState : BaseState
{
	[SerializeField] AudioAsset audioLocked;
	[SerializeField] float audioCoolDown = 0.5f;
	float timer;

	public override void Enter(GameObject obj)
	{
		timer = 0f;
		base.Enter(obj);
	}

	public override void Update(GameObject obj)
	{
		timer += Time.deltaTime;

		if (terminalObj.active)
		{
			if (timer >= audioCoolDown)
			{
				audioLocked.PlayAtPos (terminalObj.transform.position);
				timer = 0f;
			}
			
			terminalObj.active = false;
		}
		else if (!terminalObj.locked)
			terminalObj.EnterState (ChatTerminalObj.offlineState);
	}
}

public class ChatTerminalObj : FiniteStateMachine, Ivyyy.Interfaces.InteractableObject
{
	[Header ("Lara Values")]
	public GameEvent terminalUiShow;
	public float minTransitionTime = 1f;
	[Space()]
	public CinemachineVirtualCamera terminalCamera;
	public CinemachineBrain cinemachineBrain;

	public bool active;
	public bool locked;
	public LockedState lockedState = new LockedState();
	public static OfflineState offlineState = new OfflineState();
	public static PowerUpState powerUpState = new PowerUpState();
	public static OnlineState onlineState = new OnlineState();
	public static PowerDownState powerDownState = new PowerDownState();


	private void Start()
	{
		EnterState (offlineState);

		if (cinemachineBrain == null)
			cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
	}

	public void ShutDown()
	{
		active = false;
	}

	public void SetLocked (bool val)
	{
		locked = val;
	}

	public void Interact()
	{
		active = true;
	}
}

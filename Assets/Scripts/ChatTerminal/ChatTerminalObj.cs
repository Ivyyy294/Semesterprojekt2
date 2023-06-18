using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;
using Cinemachine;
using Ivyyy.StateMachine;

public class OfflineState : IState
{
	ChatTerminalObj terminalObj;

	public void Enter (GameObject obj)
	{
		terminalObj = obj.GetComponent<ChatTerminalObj>();
		terminalObj.terminalCamera.Priority = 0;
		terminalObj.eventReleasePlayer?.Raise();
	}

	public void Update (GameObject obj)
	{
		if (terminalObj.active)
			terminalObj.SetState (ChatTerminalObj.powerUpState);
	}
}

public class PowerUpState : IState
{
	ChatTerminalObj terminalObj;
	float timer = 0f;

	public void Enter (GameObject obj)
	{
		terminalObj = obj.GetComponent<ChatTerminalObj>();
		terminalObj.eventLockPlayer?.Raise();
		terminalObj.terminalCamera.Priority = 2;
		timer = 0f;
	}

	public void Update (GameObject obj)
	{
		if (timer >= terminalObj.minTransitionTime && !terminalObj.cinemachineBrain.IsBlending)
			terminalObj.SetState (ChatTerminalObj.onlineState);
		else
			timer += Time.deltaTime;
	}
}

public class PowerDownState : IState
{
	ChatTerminalObj terminalObj;
	float timer = 0f;

	public void Enter (GameObject obj)
	{
		terminalObj = obj.GetComponent<ChatTerminalObj>();
		terminalObj.terminalCamera.Priority = 0;
		timer = 0f;
	}

	public void Update (GameObject obj)
	{
		if (timer >= terminalObj.minTransitionTime && !terminalObj.cinemachineBrain.IsBlending)
			terminalObj.SetState (ChatTerminalObj.offlineState);
		else
			timer += Time.deltaTime;
	}
}

public class OnlineState : IState
{
	ChatTerminalObj terminalObj;

	public void Enter (GameObject obj)
	{
		terminalObj = obj.GetComponent<ChatTerminalObj>();
		terminalObj.terminalUiShow?.Raise();
	}

	public void Update (GameObject obj)
	{
		if (!terminalObj.active)
			terminalObj.SetState (ChatTerminalObj.powerDownState);
	}
}

public class ChatTerminalObj : MonoBehaviour, Ivyyy.Interfaces.InteractableObject
{
	[Header ("Lara Values")]
	public GameEvent eventLockPlayer;
	public GameEvent eventReleasePlayer;
	public GameEvent terminalUiShow;
	public float minTransitionTime = 1f;
	[Space()]
	public CinemachineVirtualCamera terminalCamera;
	public CinemachineBrain cinemachineBrain;

	public bool active;
	public static OfflineState offlineState = new OfflineState();
	public static PowerUpState powerUpState = new PowerUpState();
	public static OnlineState onlineState = new OnlineState();
	public static PowerDownState powerDownState = new PowerDownState();

	private IState currentState;

	private void Start()
	{
		SetState (new OfflineState());

		if (cinemachineBrain == null)
			cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
	}

	public void Update()
	{
		currentState?.Update (gameObject);
	}

	public void SetState (IState newState)
	{
		currentState = newState;
		currentState.Enter(gameObject);
	}

	public void ShutDown()
	{
		active = false;
	}

	public void Interact()
	{
		active = true;
	}
}

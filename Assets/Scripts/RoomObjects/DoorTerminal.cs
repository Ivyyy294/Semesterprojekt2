using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
using Ivyyy.StateMachine;
using Ivyyy.GameEvent;

[RequireComponent (typeof (AudioPlayer))]
public class DoorTerminal : PushdownAutomata, InteractableObject
{
	public class BaseState : IState
	{
		protected DoorTerminal doorTerminal;
		public virtual void Enter (GameObject obj)
		{
			doorTerminal = obj.GetComponent<DoorTerminal>();
		}

		public virtual void Update (GameObject obj)
		{
		}

		public virtual void Exit(GameObject obj)
		{
		}
	}

	[System.Serializable]
	public class AccessDeniedState : BaseState
	{
		[SerializeField] AudioPlayer audioPlayer;
		[SerializeField] AudioAsset audioAsset;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			audioPlayer?.Play(audioAsset);
		}

		public override void Update (GameObject obj)
		{
			if (!audioPlayer.IsPlaying())
				doorTerminal.PopState();
		}
	}

	[System.Serializable]
	public class PuckState : BaseState
	{
		[SerializeField] AudioPlayer audioPlayer;
		[SerializeField] AudioAsset audioAssets;
		[SerializeField] AudioAsset lastAudioAsset;
		int counter;
		bool audioPlayed;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			audioPlayed = false;
		}

		public override void Update (GameObject obj)
		{
			if (counter < audioAssets.ClipCount() && ! audioPlayed)
			{
				audioPlayer?.Play (audioAssets);
				audioPlayed = true;
			}
			else if (!audioPlayed)
			{
				audioPlayer?.Play (lastAudioAsset);
				audioPlayed = true;
			}
			else if (!audioPlayer.IsPlaying())
				doorTerminal.PopState();
		}

		public override void Exit(GameObject obj)
		{
			counter++;
		}
	}

	public BaseState baseState = new BaseState();
	public AccessDeniedState accessDenied = new AccessDeniedState();
	public PuckState puckState = new PuckState();

	public bool active;
	public bool locked = true;

    public void Interact()
	{
		if (locked)
		{
			if (CurrentState() == baseState)
			{
				active = false;
				PushState (puckState);
				PushState (accessDenied);
			}
		}
		else
			active = true;
	}

	private void Start()
	{
		PushState (baseState);
	}
}

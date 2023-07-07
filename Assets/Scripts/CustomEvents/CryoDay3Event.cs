using System;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;
using Ivyyy.StateMachine;

[RequireComponent (typeof (AudioPlayer))]
class CryoDay3Event : FiniteStateMachine
{
	public class BaseState : IState
	{
		protected CryoDay3Event cryoEvent;
		public virtual void Enter (GameObject obj)
		{
			cryoEvent = obj.GetComponent <CryoDay3Event>();
		}
		public virtual void Update (GameObject obj){}
		public virtual void Exit(GameObject obj) {}
	}

	[System.Serializable]
	public class Stage1 : BaseState
	{
		[SerializeField] Door cryoDoor;
		[SerializeField] AudioAsset voiceLines;
		[SerializeField] ChatTerminalObj chatTerminal;
		[SerializeField] GameEvent closeTerminal;
		[SerializeField] float voiceLineDelay = 1f;
		float timer = 0f;
		int currentIndex;
		
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			currentIndex = 0;
			cryoDoor.open = true;
			timer = 0f;
			chatTerminal.SetLocked(true);
			closeTerminal.Raise();
		}

		public override void Update(GameObject obj)
		{
			if (!cryoEvent.playerInCryoRoom)
			{
				if (currentIndex >= voiceLines.ClipCount() && !cryoEvent.audioPlayer.IsPlaying())
					cryoEvent.EnterState (cryoEvent.idleState);
				else if (!cryoEvent.audioPlayer.IsPlaying())
				{
					if (timer < voiceLineDelay)
						timer += Time.deltaTime;
					else
					{
						cryoEvent.audioPlayer.Play(voiceLines);
						currentIndex++;
						timer = 0f;
					}
				}
			}
			else
				cryoEvent.EnterState (cryoEvent.stage2);
		}

		public override void Exit(GameObject obj)
		{
			cryoDoor.open = false;
			chatTerminal.SetLocked(false);
		}
	}

	[System.Serializable]
	public class Stage2 : BaseState
	{
		[SerializeField] Door cryoDoor;
		[SerializeField] InteractableCamera cryoChair;
		[SerializeField] Room room;
		[SerializeField] AudioAsset voiceLinesTrapped;
		[SerializeField] BlackBoardProperty testProperty;
		[SerializeField] bool forceGoodEnding = false;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			cryoEvent.audioPlayer.Play (voiceLinesTrapped);
			cryoDoor.open = false;
		}

		public override void Update(GameObject obj)
		{
			 if (cryoChair.IsActive())
			{
				BlackBoardProperty checkValue = BlackBoard.Me().GetPropertyByName (testProperty.name);

				if (checkValue.Compare (testProperty) || forceGoodEnding)
					room.EnterEndingCryoGood();
				else
					room.EnterEndingCryoBad();

				cryoEvent.EnterState(cryoEvent.idleState);
			}
		}
	}

	public BaseState idleState = new BaseState();
	public Stage1 stage1 = new Stage1();
	public Stage2 stage2 = new Stage2();
	AudioPlayer audioPlayer;
	bool playerInCryoRoom = false;

	public void Activate()
	{
		if (currentState == idleState)
		{
			EnterState (stage1);
			playerInCryoRoom = false;
		}
	}

	public void SetPlayerInCryoRoom()
	{
		playerInCryoRoom = true;
	}

	private void Start()
	{
		EnterState (idleState);
		audioPlayer = GetComponent <AudioPlayer>();
	}
}


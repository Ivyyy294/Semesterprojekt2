using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;
using Ivyyy.Interfaces;
using Ivyyy.GameEvent;
using UnityEngine.UI;
using Ivyyy.SaveGameSystem;

public class Room : FiniteStateMachine
{
	public class BaseState : IState
	{
		protected Room room;
		public virtual void Enter (GameObject obj)
		{
			room = obj.GetComponent <Room>();
		}

		public virtual void Update (GameObject obj)
		{
		}

		public virtual void Exit(GameObject obj)
		{
		}
	}

	[System.Serializable]
	public class InitValuesState : BaseState
	{
		[SerializeField] Transform PlayerSpawnPos;
		[SerializeField] LightController lightController;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);

			Player.Me().Lock();
			Player.Me().transform.position = PlayerSpawnPos.position;
			Player.Me().transform.forward = PlayerSpawnPos.forward;
			lightController.EnterNormalState();
		}

		public override void Update(GameObject obj)
		{
			room.cryoDoor.SetOpen (true);
			room.EnterState (room.choseDayState);
		}
	}

	[System.Serializable]
	public class ChoseDayState : BaseState
	{
		[SerializeField] AnimationCurve animationCurve;
		[SerializeField] Image image;
		float timer;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			timer = 0f;
			image.gameObject.SetActive (true);
		}

		public override void Update (GameObject obj)
		{
			if (timer < animationCurve.keys[animationCurve.length -1].time)
			{
				Color color = image.color;
				color.a = animationCurve.Evaluate (timer);
				image.color = color;

				timer += Time.deltaTime;
			}
			else
				room.EnterState(room.dayState);
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().Unlock();
			image.gameObject.SetActive (false);
		}

	}

	[System.Serializable]
	public class DayState : BaseState
	{
		[SerializeField] string propertyName;
		[SerializeField] int threshold;
		public bool done;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
		}

		public override void Update (GameObject obj)
		{
			if (done)
				room.EnterState (room.nightState);
		}

		public override void Exit(GameObject obj)
		{
			done = false;
		}
	}

	[System.Serializable]
	public class NightState : BaseState
	{
		[SerializeField] GameObject nextDayTrigger;
		[SerializeField] AudioAsset audioAsset;
		[SerializeField] LightController lightController;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			nextDayTrigger.SetActive (true);
			Player.Me().BlockInteractions (true);
			lightController.EnterNightState();
			audioAsset?.Play();
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().BlockInteractions (false);
		}
	}

	[System.Serializable]
	public class TransitionState : BaseState
	{
		[SerializeField] Image image;
		[SerializeField] GameObject txt;
		[SerializeField] AnimationCurve animationCurve;
		[SerializeField] GameObject nextDayTrigger;
		[SerializeField] float txtTime;
		float timer;
		float timerTxt;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			room.cryoDoor.SetOpen (false);
			Player.Me().Lock();
			timer = 0f;
			timerTxt = 0f;
			image.gameObject.SetActive (true);
		}

		public override void Update(GameObject obj)
		{
			if (timer <= animationCurve.keys[animationCurve.length -1].time)
			{
				Color color = image.color;
				color.a = animationCurve.Evaluate (timer);
				image.color = color;

				timer += Time.deltaTime;
			}
			else if (timerTxt <= txtTime)
			{
				txt.SetActive (true);
				timerTxt += Time.deltaTime;
			}
			else 
				room.EnterState (room.initValuesState);
		}

		public override void Exit(GameObject obj)
		{
			nextDayTrigger.SetActive (false);
			txt.SetActive (false);
		}
	}

	[Header ("Room Objects")]
	public CryoDoor cryoDoor;
	
	[Header ("Room States")]
	public ChoseDayState choseDayState = new ChoseDayState();
	public NightState nightState = new NightState();
	public DayState dayState = new DayState();
	public TransitionState transitionState = new TransitionState();
	public InitValuesState initValuesState = new InitValuesState();

	//public Player player;

	public void EnterNight()
	{
		dayState.done = true;
	}

	public void NextDay()
	{
		if (currentState == nightState)
			EnterState (transitionState);
	}

	protected override void Update()
	{
		if (currentState == null)
		{
			if (SaveGameManager.Me().LoadGameScheduled)
			{
				SaveGameManager.Me().LoadGameState();
				EnterState (choseDayState);
			}
			else
				EnterState (initValuesState);
		}
		else
			base.Update();
	}
}

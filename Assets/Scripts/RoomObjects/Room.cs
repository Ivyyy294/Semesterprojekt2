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
		[SerializeField] GameObject personalObjects;
		[SerializeField] AnimationCurve animationCurve;
		[SerializeField] Image image;
		float timer;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			timer = 0f;
			image.gameObject.SetActive (true);

			personalObjects.SetActive (room.currentDay == CurrentDay.DAY2);
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
			{
				if (room.currentDay == CurrentDay.DAY1)
					room.EnterState(room.day1State);
				else if (room.currentDay == CurrentDay.DAY2)
					room.EnterState(room.day2State);
				else
					room.EnterState(room.day3State);
			}
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().Unlock();
			image.gameObject.SetActive (false);
		}
	}

	[System.Serializable]
	public class DayState : BaseState , IGameEventListener
	{
		[SerializeField] GameEvent exitEvent;
		bool done;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			exitEvent?.RegisterListener(this);
			done = false;
		}

		public override void Update(GameObject obj)
		{
			if (done)
				room.EnterState (room.nightState);
		}

		public override void Exit(GameObject obj)
		{
			exitEvent?.UnregisterListener (this);
		}

		public void OnEventRaised()
		{
			done = true;
		}
	}

	[System.Serializable]
	public class NightState : BaseState, IGameEventListener
	{
		[SerializeField] GameEvent exitEvent;
		[SerializeField] GameObject nextDayTrigger;
		[SerializeField] AudioAsset audioAsset;
		[SerializeField] LightController lightController;
		bool done = false;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			done = false;
			exitEvent?.RegisterListener (this);
			nextDayTrigger.SetActive (true);
			Player.Me().BlockInteractions (true);
			lightController.EnterNightState();
			audioAsset?.Play();
		}

		public override void Update(GameObject obj)
		{
			if (done)
				room.EnterState (room.transitionState);
		}

		public override void Exit(GameObject obj)
		{
			exitEvent.UnregisterListener (this);
			Player.Me().BlockInteractions (false);
		}

		public void OnEventRaised()
		{
			done = true;
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
			{
				if (room.currentDay == CurrentDay.DAY1)
					room.currentDay = CurrentDay.DAY2;
				else if (room.currentDay == CurrentDay.DAY2)
					room.currentDay = CurrentDay.DAY3;

				room.EnterState (room.initValuesState);
			}
		}

		public override void Exit(GameObject obj)
		{
			nextDayTrigger.SetActive (false);
			txt.SetActive (false);
		}
	}

	public enum CurrentDay
	{
		DAY1,
		DAY2,
		DAY3
	}

	public CurrentDay currentDay = CurrentDay.DAY1;

	[Header ("Room Objects")]
	public CryoDoor cryoDoor;
	
	[Header ("Room States")]
	public ChoseDayState choseDayState = new ChoseDayState();
	public NightState nightState = new NightState();
	public DayState day1State = new DayState();
	public DayState day2State = new DayState();
	public DayState day3State = new DayState();
	public TransitionState transitionState = new TransitionState();
	public InitValuesState initValuesState = new InitValuesState();

	//public Player player;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;
using Ivyyy.Interfaces;
using Ivyyy.GameEvent;
using UnityEngine.UI;
using Ivyyy.SaveGameSystem;

public class Room : MonoBehaviour
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
	public class FadeInState : BaseState
	{
		[SerializeField] AnimationCurve animationCurve;
		[SerializeField] Image image;
		float timer;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			timer = 0f;
			image.gameObject.SetActive(true);
		}

		public override void Update(GameObject obj)
		{
			if (timer < animationCurve.keys[animationCurve.length - 1].time)
			{
				Color color = image.color;
				color.a = animationCurve.Evaluate(timer);
				image.color = color;

				timer += Time.deltaTime;
			}
			else
				room.PopState();
		}

		public override void Exit(GameObject obj)
		{
			image.gameObject.SetActive(false);
		}
	}

	[System.Serializable]
	public class WakeUpCryo : BaseState
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
			room.PushState (room.fadeInState);
			room.cryoDoor.SetOpen (true);
		}

		public override void Update(GameObject obj)
		{
			room.PopState();
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().Unlock();
		}
	}

	[System.Serializable]
	public class ChoseDayState : BaseState
	{
		public override void Update (GameObject obj)
		{
			if (room.currentDay == CurrentDay.DAY1)
				room.PushState (room.day1State);
			else if (room.currentDay == CurrentDay.DAY2)
				room.PushState (room.day2State);
			else
				room.PushState (room.day3State);
		}
	}

	[System.Serializable]
	public class Day1State : BaseState , IGameEventListener
	{
		[SerializeField] GameEvent nightEvent;
		bool done;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			done = false;
			nightEvent?.RegisterListener(this);
			room.PushState (room.wakeUpCryo);
		}

		public override void Update(GameObject obj)
		{
			if (done)
				room.SwapState (room.nightState);
		}

		public override void Exit(GameObject obj)
		{
			nightEvent?.UnregisterListener (this);
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
				room.SwapState (room.transitionState);
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

				room.PopState();
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
	public bool night;

	[Header ("Room Objects")]
	public CryoDoor cryoDoor;
	
	[Header ("Room States")]
	public ChoseDayState choseDayState = new ChoseDayState();
	public FadeInState fadeInState = new FadeInState();
	public WakeUpCryo wakeUpCryo = new WakeUpCryo();
	public Day1State day1State = new Day1State();
	public Day1State day2State = new Day1State();
	public Day1State day3State = new Day1State();
	public NightState nightState = new NightState();
	public TransitionState transitionState = new TransitionState();

	private Stack <IState> states = new Stack<IState>();

	public void SwapState (IState newState)
	{
		PopState();
		PushState (newState);
	}

	public void PushState (IState newState)
	{
		states.Push (newState);
		states.Peek().Enter(gameObject);
	}

	public void PopState()
	{
		states.Pop().Exit(gameObject);
	}

	//public Player player;
	protected void Update()
	{
		if (states.Count == 0)
		{
			if (SaveGameManager.Me().LoadGameScheduled)
				SaveGameManager.Me().LoadGameState();
			
			PushState (choseDayState);
		}
		else
			states.Peek().Update(gameObject);
	}
}

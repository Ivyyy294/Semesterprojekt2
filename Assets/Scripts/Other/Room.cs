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
	public class WakeUpState : BaseState
	{
		[SerializeField] AnimationCurve animationCurve;
		float timer;
		[SerializeField] Transform PlayerSpawnPos;
		[SerializeField] Image image;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);

			timer = 0f;
			image.gameObject.SetActive (true);

			Player.Me().Lock();

			if (!SaveGameManager.Me().LoadGameScheduled)
			{
				Player.Me().transform.position = PlayerSpawnPos.position;
				Player.Me().transform.forward = PlayerSpawnPos.forward;
			}
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

	public class DayState : BaseState
	{
		public override void Update (GameObject obj)
		{
			if (room.night == true)
				room.EnterState (room.nightState);
		}
	}

	[System.Serializable]
	public class NightState : BaseState
	{
		[SerializeField] GameObject[] interactableObjects;
		[SerializeField] GameObject nextDayTrigger;
		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			SetInteractableObjects (false);
		}

		public override void Exit(GameObject obj)
		{
			SetInteractableObjects (true);
			room.night = false;
		}

		void SetInteractableObjects (bool enabled)
		{
			nextDayTrigger?.SetActive (!enabled);

			foreach (var i in interactableObjects)
			{
				MonoBehaviour behaviour = i.GetComponentInChildren<InteractableObject>() as MonoBehaviour;

				if (behaviour != null)
					behaviour.enabled = enabled;
			}
		}
	}

	[System.Serializable]
	public class TransitionState : BaseState
	{
		[SerializeField] Image image;
		[SerializeField] GameObject txt;
		[SerializeField] AnimationCurve animationCurve;
		[SerializeField] float txtTime;
		float timer;
		float timerTxt;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
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
				room.EnterState (room.wakeUpState);
		}

		public override void Exit(GameObject obj)
		{
			txt.SetActive (false);
		}
	}

	public bool night = false;
	[Header ("Lara Values")]
	public WakeUpState wakeUpState = new WakeUpState();
	public NightState nightState = new NightState();
	public DayState dayState = new DayState();
	public TransitionState transitionState = new TransitionState();

	public void NextDay()
	{
		if (currentState == nightState)
			EnterState (transitionState);
	}

    // Start is called before the first frame update
    void Start()
    {
        EnterState (wakeUpState);
    }
}

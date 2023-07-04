using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;
using Ivyyy.Interfaces;
using Ivyyy.GameEvent;
using UnityEngine.UI;
using Ivyyy.SaveGameSystem;
using Cinemachine;
using TMPro;

public class Room : PushdownAutomata
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
	public class FadeOutState : BaseState
	{
		[SerializeField] Image image;
		[SerializeField] GameObject txt;
		[SerializeField] AnimationCurve animationCurve;
		[SerializeField] float txtTime;
		float timer;
		float timerTxt;

		public void SetText (string text)
		{
			TextMeshProUGUI tmp = txt.GetComponent <TextMeshProUGUI>();
			tmp.text = text;
		}

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
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
				room.PopState();
		}

		public override void Exit(GameObject obj)
		{
			txt.SetActive (false);
		}
	}

	//ToDo: Let player spawn sitting on chair
	[System.Serializable]
	public class WakeUpCryo : BaseState
	{
		[SerializeField] Transform PlayerSpawnPos;
		[SerializeField] InteractableCamera cryoCam;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			
			//Spawn player sitting on cryo chair
			if(!cryoCam.IsActive())
				cryoCam.EnterState (cryoCam.spawnState);

			Player.Me().Lock();
			Player.Me().transform.position = PlayerSpawnPos.position;
			Player.Me().transform.forward = PlayerSpawnPos.forward;
			room.PushState (room.fadeInState);
			room.cryoDoor.SetOpen (true);
		}

		public override void Update(GameObject obj)
		{
			room.PopState();
		}
	}

	[System.Serializable]
	//ToDo: Let player spawn laying on bed, even when loading game
	public class WakeUpBed : BaseState
	{
		[SerializeField] Transform PlayerSpawnPos;
		[SerializeField] InteractableCamera bed;
		[SerializeField] AudioAsset audioAsset;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);

			//Spawn player laying on bed
			if(!bed.IsActive())
				bed.EnterState (bed.spawnState);

			Player.Me().Lock();
			Player.Me().transform.position = PlayerSpawnPos.position;
			Player.Me().transform.forward = PlayerSpawnPos.forward;

			room.PushState (room.fadeInState);
		}

		public override void Update(GameObject obj)
		{
			audioAsset?.PlayAtPos(bed.transform.position);
			room.PopState();
		}

		public override void Exit(GameObject obj)
		{
			bed.activeState.SetLocked (false);
		}
	}

	[System.Serializable]
	public class ChoseDayState : BaseState
	{
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
		}

		public override void Update (GameObject obj)
		{
			Player.Me().cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;

			if (room.currentDay == CurrentDay.DAY1)
				room.PushState (room.day1State);
			else if (room.currentDay == CurrentDay.DAY2)
				room.PushState (room.day2State);
			else
				room.PushState (room.day3State);
		}
	}

	[System.Serializable]
	public class NightState : BaseState
	{
		[SerializeField] InteractableCamera bed;
		[SerializeField] AudioAsset audioAsset;
		[SerializeField] LightController lightController;
		[SerializeField] string tag;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			Player.Me().BlockInteractions (true, tag);
			lightController.EnterNightState();
			audioAsset?.Play();
			bed.activeState.SetLocked (true);
		}

		public override void Update(GameObject obj)
		{
			if (bed != null && bed.IsActive())
				room.SwapState (room.transitionState);
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().BlockInteractions (false, "");
		}
	}

	[System.Serializable]
	public class TransitionState : BaseState
	{
		[SerializeField] string propertyName;
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			Player.Me().Lock();
			room.PushState(room.fadeOutState);
		}

		public override void Update(GameObject obj)
		{
			if (!string.IsNullOrEmpty(propertyName))
			{
				string guid = BlackBoard.Me().GetGuidByName (propertyName);
				BlackBoard.Me().EditValue (guid, BlackBoard.EditTyp.INCREASE, 1);

				Debug.Log ("DayCounter: " + BlackBoard.Me().GetProperty(guid).iVal.ToString());
			}

			if (room.currentDay == CurrentDay.DAY1)
				room.currentDay = CurrentDay.DAY2;
			else if (room.currentDay == CurrentDay.DAY2)
				room.currentDay = CurrentDay.DAY3;

			SaveGameManager.Me().SaveGameState();

			room.PopState();
		}
	}
	
	[System.Serializable]
	public class CryoEventState : BaseState
	{
		//public override void Enter(GameObject obj)
		//{
		//	base.Enter(obj);
		//}

		//public override void Update(GameObject obj)
		//{
		//	if (room.currentDay == CurrentDay.DAY1)
		//		room.currentDay = CurrentDay.DAY2;
		//	else if (room.currentDay == CurrentDay.DAY2)
		//		room.currentDay = CurrentDay.DAY3;

		//	SaveGameManager.Me().SaveGameState();

		//	room.PopState();
		//}
	}

	[System.Serializable]
	public class Day1State : BaseState , IGameEventListener
	{
		[SerializeField] LightController lightController;
		[SerializeField] GameEvent nightEvent;
		bool done;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			done = false;
			nightEvent?.RegisterListener(this);
			lightController.EnterNormalState();
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
	public class Day2State : BaseState
	{
		[SerializeField] LightController lightController;
		[SerializeField] string nameProgressProperty;
		[SerializeField] int checkValue;
		[SerializeField] PuckTerminal terminal;
		[SerializeField] CryoDoor cryoDoor;
		[SerializeField] GameObject areaEvent;
		BlackBoardProperty property;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			lightController.EnterNormalState();
			room.PushState (room.wakeUpBed);
			terminal.SetChatVisible (3, false);
			terminal.SetChatAvailable (0, true);
			terminal.SetChatAvailable (1, true);
			terminal.SetChatAvailable (2, true);
			terminal.SetActiveChat (0);
			cryoDoor.SpawnOpen();
			areaEvent.SetActive(true);
			property = BlackBoard.Me().GetPropertyByName (nameProgressProperty);
		}

		public override void Update(GameObject obj)
		{
			if (property.Compare (new BlackBoardProperty {comparisonTyp = BlackBoardProperty.ComparisonTyp.EQUAL, iVal = checkValue}))
				room.SwapState (room.nightState);
		}

		public override void Exit(GameObject obj)
		{
			areaEvent.SetActive(false);
		}
	}

	[System.Serializable]
	public class Day3State : BaseState
	{
		[SerializeField] LightController lightController;
		[SerializeField] GameObject cryoDoorTrigger;
		[SerializeField] GameObject personalItems;
		[SerializeField] GameObject cryoEvent;
		[SerializeField] LightBulbEvent lightBulbEvent;
		[SerializeField] PuckTerminal terminal;
		[SerializeField] HermiaVoiceLine hermiaVoiceEvent;
		[SerializeField] float hermiaEventDelay = 4f;
		[SerializeField] CryoDoor cryoDoor;
		float timer = 0f;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			room.cryoDoor.SetOpen (true);
			personalItems.SetActive (false);
			cryoDoorTrigger.SetActive (false);
			cryoEvent.SetActive (true);
			lightBulbEvent.DisableLight (true);
			lightController.EnterDay3State();
			terminal.SetChatVisible (3, true);
			terminal.SetChatAvailable (0, false);
			terminal.SetChatAvailable (1, false);
			terminal.SetChatAvailable (2, false);
			terminal.SetChatAvailable (3, true);
			terminal.SetActiveChat (3);
			cryoDoor.SpawnOpen();
			timer = 0f;
			room.PushState (room.wakeUpBed);
		}

		public override void Update(GameObject obj)
		{
			if (timer < hermiaEventDelay)
				timer += Time.deltaTime;
			else if (!hermiaVoiceEvent.IsActive())
				hermiaVoiceEvent.Activate();
		}

		public override void Exit(GameObject obj)
		{
			cryoDoorTrigger.SetActive (true);
		}
	}

	[System.Serializable]
	public class EndingCryoGood : BaseState
	{
		[SerializeField] GameEvent creditsEvent;
		[SerializeField] DoorTerminal doorTerminal;
		[SerializeField] string tag;
		bool audioPlayed = false;
		bool done = false;
		bool doorSoundPlayed = false;

		[SerializeField] AudioAsset audioAssetArrived;
		[SerializeField] AudioAsset audioAssetDoor;
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			audioPlayed = false;
			done = false;
			doorSoundPlayed = false;
			Player.Me().Lock();
			Player.Me().BlockInteractions (true, tag);
			doorTerminal.locked = false;
			room.fadeOutState.SetText ("");
			//Queue Fade In and Out effect
			room.PushState (room.wakeUpCryo);
			room.PushState (room.fadeOutState);
		}

		public override void Update(GameObject obj)
		{
			if (!audioPlayed)
			{
				audioAssetArrived?.Play();
				audioPlayed = true;
			}
			else if (!done && doorTerminal.active)
			{
				room.PushState (room.fadeOutState);
				done = true;
			}
			else if (done && !doorSoundPlayed)
			{
				doorSoundPlayed = true;
				audioAssetDoor?.Play();
			}
			else if (done)
				creditsEvent?.Raise();
		}
	}

	[System.Serializable]
	public class EndingCryoBad : BaseState
	{
		[SerializeField] GameEvent creditsEvent;
		bool audioPlayed = false;

		[SerializeField] AudioAsset audioDying;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			audioPlayed = false;
			Player.Me().Lock();
			room.fadeOutState.SetText ("");
			//Queue Fade In and Out effect
			room.PushState (room.fadeOutState);
		}

		public override void Update(GameObject obj)
		{
			if (!audioPlayed)
			{
				audioDying?.Play();
				audioPlayed = true;
			}
			else
				creditsEvent?.Raise();
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
	//DayStates
	public Day1State day1State = new Day1State();
	public Day2State day2State = new Day2State();
	public Day3State day3State = new Day3State();

	//SubStates
	public ChoseDayState choseDayState = new ChoseDayState();
	public FadeInState fadeInState = new FadeInState();
	public FadeOutState fadeOutState = new FadeOutState();
	public WakeUpCryo wakeUpCryo = new WakeUpCryo();
	public WakeUpBed wakeUpBed = new WakeUpBed();
	public NightState nightState = new NightState();
	public TransitionState transitionState = new TransitionState();

	//Endings
	public EndingCryoGood endingCryoGood = new EndingCryoGood();
	public EndingCryoBad endingCryoBad = new EndingCryoBad();

	//Public Functions
	public void EnterEndingCryoGood()
	{
		if (CurrentState() == day3State)
			SwapState (endingCryoGood);
	}
	public void EnterEndingCryoBad()
	{
		if (CurrentState() == day3State)
			SwapState (endingCryoBad);
	}

	//Private Functions
	private void Start()
	{
		PushState (choseDayState);
	}

	//public Player player;
	protected override void Update()
	{
		if (SaveGameManager.Me().LoadGameScheduled)
			SaveGameManager.Me().LoadGameState();
			
		base.Update();
	}
}

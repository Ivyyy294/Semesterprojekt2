using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ivyyy.Interfaces;
using Ivyyy.StateMachine;


public class Letter : FiniteStateMachine, InteractableObject
{
	public class BaseState : IState
	{
		protected Letter letter;
		public virtual void Enter (GameObject obj)
		{
			letter = obj.GetComponent<Letter>();
		}
		public virtual void Update (GameObject obj)
		{}
		public virtual void Exit(GameObject obj){}
	}

	[System.Serializable]
	public class FadeState : BaseState
	{
		[SerializeField] protected AnimationCurve curve;
		[SerializeField] protected Image imgFadeBlack;
		protected float timer;

		protected void ChangeAlpha (float timer, AnimationCurve curve)
		{
			if (imgFadeBlack != null)
			{
				Color color = imgFadeBlack.color;
				color.a = curve.Evaluate (timer);
				imgFadeBlack.color = color;
			}
		}
	}

	[System.Serializable]
	public class FadeInState : FadeState
	{
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			Player.Me().Lock();
			timer = 0f;
			imgFadeBlack.gameObject.SetActive(true);
		}

		public override void Update(GameObject obj)
		{
			if (timer <= curve.keys[curve.keys.Length -1].time)
			{
				ChangeAlpha (timer, curve);
				timer += Time.deltaTime;
			}
			else
				letter.EnterState (letter.activeState);
		}
	}

	[System.Serializable]
	public class FadeOutState : FadeState
	{
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			timer = 0f;
		}

		public override void Update(GameObject obj)
		{
			if (timer <= curve.keys[curve.keys.Length -1].time)
			{
				ChangeAlpha (timer, curve);
				timer += Time.deltaTime;
			}
			else
				letter.EnterState (letter.baseState);
		}

		public override void Exit(GameObject obj)
		{
			imgFadeBlack.gameObject.SetActive(false);
		}
	}

	[System.Serializable]
	public class ActiveState : BaseState
	{
		[SerializeField] GameObject letterObj;
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			letterObj.SetActive(true);
		}

		public override void Update(GameObject obj)
		{
			if (Input.GetKey(KeyCode.F))
				letter.EnterState(letter.fadeOutState);
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().Unlock();
			letterObj.SetActive(false);
		}
	}

	public BaseState baseState = new BaseState();
	public FadeInState fadeInState = new FadeInState();
	public FadeOutState fadeOutState = new FadeOutState();
	public ActiveState activeState = new ActiveState();

	public void Interact()
	{
		EnterState (fadeInState);
	}

	private void Start()
	{
		EnterState (baseState);
	}
}

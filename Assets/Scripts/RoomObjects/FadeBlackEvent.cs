using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ivyyy.Interfaces;
using Ivyyy.StateMachine;

[RequireComponent (typeof(AudioPlayer))]
public class FadeBlackEvent : FiniteStateMachine, InteractableObject
{
	public class BaseState : IState
	{
		protected FadeBlackEvent parent;
		public virtual void Enter (GameObject obj)
		{
			parent = obj.GetComponent <FadeBlackEvent>();
		}
		public virtual void Update (GameObject obj){}
		public virtual void Exit(GameObject obj){ }
	}

	public class FadeInState : BaseState
	{
		float timer;
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			timer = 0f;
			parent.img.gameObject.SetActive(true);
			Player.Me().Lock();
		}

		public override void Update(GameObject obj)
		{
			parent.ChangeAlpha (timer, parent.fadeInCurve);
			timer += Time.deltaTime;

			if (timer > parent.fadeInCurve.keys[parent.fadeInCurve.keys.Length -1].time)
				parent.EnterState(parent.activeState);
		}
	}

	public class FadeOutState : BaseState
	{
		float timer;
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			timer = 0f;
			parent.ChangeAlpha (timer, parent.fadeOutCurve);
		}

		public override void Update(GameObject obj)
		{
			parent.ChangeAlpha (timer, parent.fadeOutCurve);
			timer += Time.deltaTime;

			if (timer > parent.fadeOutCurve.keys[parent.fadeOutCurve.keys.Length -1].time)
				parent.EnterState(parent.baseState);
		}

		public override void Exit(GameObject obj)
		{
			parent.img.gameObject.SetActive(false);
		}
	}

	public class ActiveState : BaseState
	{
		float timer;
		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
			timer = 0f;
			parent.audioPlayer.Play();
		}

		public override void Update(GameObject obj)
		{
			parent.ChangeAlpha (1f);

			if (timer < 0.5f)
				timer += Time.deltaTime;
			else if (!parent.audioPlayer.IsPlaying())
				parent.EnterState(parent.fadeOutState);
		}

		public override void Exit(GameObject obj)
		{
			Player.Me().Unlock();
		}
	}

	public Image img;
	public AnimationCurve fadeInCurve;
	public AnimationCurve fadeOutCurve;
	AudioPlayer audioPlayer;

	public FadeInState fadeInState = new FadeInState();
	public FadeOutState fadeOutState = new FadeOutState();
	public ActiveState activeState = new ActiveState();
	public BaseState baseState = new BaseState();

	public void Interact()
	{
		EnterState (fadeInState);
	}

	// Start is called before the first frame update
    void Start()
    {
		EnterState (baseState);
		audioPlayer = GetComponent<AudioPlayer>();
    }

  	void ChangeAlpha (float timer, AnimationCurve curve)
	{
		ChangeAlpha (curve.Evaluate (timer));
	}

	void ChangeAlpha (float val)
	{
		if (img != null)
		{
			Color color = img.color;
			color.a = val;
			Debug.Log (color.a);
			img.color = color;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.StateMachine;

public class InteractTextOverlay : FiniteStateMachine
{
	public abstract class InteractTextOverlayBaseState : IState
	{
		protected TextMeshProUGUI txt;
		protected InteractTextOverlay overlay;
		protected Color color;

		public virtual void Enter (GameObject obj)
		{
			overlay = obj.GetComponent <InteractTextOverlay>();
			txt = overlay?.GetTextElement();

			if (txt != null)
				color = txt.color;
		}

		public abstract void Update (GameObject obj);
		public void Exit(GameObject obj) {}
	}

	[System.Serializable]
	public class ShowState : InteractTextOverlayBaseState
	{
		[SerializeField] float speed = 1f;

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);
		}

		public override void Update(GameObject obj)
		{
			if (overlay != null && !overlay.GetShow())
				overlay.EnterState (overlay.hideState);
			else if (color.a <= 1f)
			{
				color.a = Mathf.MoveTowards (color.a, 1f, Time.deltaTime / speed);
				txt.color = color;
			}
		}
	}

	[System.Serializable]
	public class HideState : InteractTextOverlayBaseState
	{
		[SerializeField] float speed = 1f;

		public override void Update(GameObject obj)
		{
			if (overlay != null && overlay.GetShow())
				overlay.EnterState (overlay.showState);
			else if (color.a > 0f)
			{
				color.a = Mathf.MoveTowards (color.a, 0f, Time.deltaTime / speed);
				txt.color = color;
			}
		}
	}

	[SerializeField] TextMeshProUGUI txtInteract;
	bool show;
	public ShowState showState = new ShowState();
	public HideState hideState = new HideState();

	//Public Functions
	public void Show(bool val, string text = "[F]")
	{
		txtInteract.text = text;
		show = val;
	}
	
	public TextMeshProUGUI GetTextElement() {return txtInteract;}
	public bool GetShow() {return show;}

	//Private Functions
    void Start()
    {
		if (txtInteract == null)
			Debug.LogError("Missing txtInteract reference!");
		else
		{
			//Start with invisible text
			Color tmp = txtInteract.color;
			tmp.a = 0f;
			txtInteract.color = tmp;
		}

		EnterState (showState);
    }
}

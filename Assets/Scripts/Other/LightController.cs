using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;

public class LightController : MonoBehaviour
{
	public abstract class BaseState : IState
	{
		protected LightController lightController;

		public virtual void Enter (GameObject obj)
		{
			lightController = obj.GetComponent <LightController>();
		}

		public abstract void Update (GameObject obj);
	}

	public class NormalState : BaseState
	{
		struct LightSettings
		{
			public float intensity;
		}

		private List <LightSettings> lightSettings = new List <LightSettings>();

		public override void Enter(GameObject obj)
		{
			base.Enter(obj);

			lightSettings.Clear();

			foreach (var i in lightController.lights)
				lightSettings.Add (new LightSettings {intensity = i.intensity});
		}

		public override void Update(GameObject obj)
		{
			//Sets lightning back to normal
			for (int i = 0; i < lightController.lights.Count; ++i)
			{
				Light light = lightController.lights[i];
				LightSettings settings = lightSettings[i];

				if (light.intensity != settings.intensity)
					light.intensity = Mathf.MoveTowards (light.intensity, settings.intensity, Time.deltaTime);
			}
		}
	}

	[System.Serializable]
	public class AnnouncementState : BaseState
	{
		public float targetIntensity;
		public float transitionTime;

		public override void Update(GameObject obj)
		{
			foreach (var i in lightController.lights)
			{
				if (i.intensity != targetIntensity)
					i.intensity = Mathf.MoveTowards (i.intensity, targetIntensity, Time.deltaTime / transitionTime);
			}
		}
	}

	public List <Light> lights;
	public NormalState normalState = new NormalState();
	public AnnouncementState announcementState = new AnnouncementState();
	private Stack <BaseState> stateStack = new Stack<BaseState>();

	//Public Functions
	public void PushState (BaseState newState)
	{
		stateStack.Push (newState);
		stateStack.Peek().Enter(gameObject);
	}

	public void PopState()
	{
		stateStack.Pop();
	}

	public void EnterNormalState()
	{
		while (stateStack.Peek() != normalState)
			stateStack.Pop();
	}

	public void EnterAnnouncementState()
	{
		PushState (announcementState);
	}

	//Private Functions
    // Start is called before the first frame update
    void Start()
    {
        PushState (normalState);
    }

    // Update is called once per frame
    void Update()
    {
        stateStack.Peek().Update (gameObject);
    }
}

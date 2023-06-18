using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;
using Ivyyy.GameEvent;

[RequireComponent (typeof(AudioPlayer))]
public class CoffeMachine : MonoBehaviour
{
	public abstract class BaseState : Ivyyy.StateMachine.IState
	{
		protected CoffeMachine coffeMachine;

		public virtual void Enter (GameObject obj)
		{
			coffeMachine = obj.GetComponent <CoffeMachine>();
		}

		public virtual void Update  (GameObject obj)
		{ }
	}

	public class IdleState : BaseState
	{
	}

	public class ActiveState : BaseState
	{
		public override void Update(GameObject obj)
		{
			coffeMachine.SetState (coffeMachine.idleState);
		}
	}

	[System.Serializable]
	public class PuckState : BaseState
	{
		public AudioAsset audioAsset;
		public GameEvent closeTerminalEvent;

		public override void Enter (GameObject obj)
		{
			base.Enter(obj);
			closeTerminalEvent?.Raise();
			coffeMachine.audioPlayer.Play (audioAsset);
		}

		public override void Update(GameObject obj)
		{
			coffeMachine.SetState (coffeMachine.activeState);
		}
	}

	public PuckState puckState = new PuckState();
	public IdleState idleState = new IdleState();
	public ActiveState activeState = new ActiveState();

	private AudioPlayer audioPlayer;
	private BaseState currentState;

	//Public Functions
	public void EnterPuckState()
	{
		SetState (puckState);
	}

	//Private Functions
	void SetState (BaseState newState)
	{
		currentState = newState;
		currentState.Enter(gameObject);
	}
    // Start is called before the first frame update
    void Start()
    {
		audioPlayer = GetComponent <AudioPlayer>();
		SetState (idleState);
    }

    // Update is called once per frame
    void Update()
    {
		currentState.Update (gameObject);
    }
}

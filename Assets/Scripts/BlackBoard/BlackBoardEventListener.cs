using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class BlackBoardEventListener : MonoBehaviour, IGameEventListener
{
	[SerializeField] GameEvent gameEvent;

	[SerializeField] string propertyName;
	[SerializeField] BlackBoard.EditTyp editTyp;
	[SerializeField] int value;

	public void OnEventRaised()
	{
		BlackBoard.Me().EditValue (propertyName, editTyp, value);
	}

	private void OnEnable()
	{
		gameEvent.RegisterListener(this);
	}

	private void OnDisable()
	{
		gameEvent.UnregisterListener(this);
	}
}

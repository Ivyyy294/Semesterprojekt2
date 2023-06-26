using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
using UnityEngine.Events;

public class NextDayTrigger : MonoBehaviour, InteractableObject
{
	public UnityEvent nextDayEvent;

	public void Interact()
	{
		nextDayEvent?.Invoke();
	}
}

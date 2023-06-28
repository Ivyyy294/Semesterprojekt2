using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
using UnityEngine.Events;
using Ivyyy.GameEvent;

public class NextDayTrigger : MonoBehaviour, InteractableObject
{
	[SerializeField] GameEvent nextDayEvent;
	[SerializeField] GameObject vCamera;
	[SerializeField] float minAnimationtime = 0.5f;
	float timer;
	bool done = false;

	public void Interact()
	{
		vCamera.SetActive(true);
		timer = 0f;
	}

	private void OnEnable()
	{
		vCamera.SetActive(false);
		done = false;
	}

	private void Update()
	{
		if (!done)
		{
			if (timer < minAnimationtime)
				timer += Time.deltaTime;
			else if (vCamera.activeInHierarchy && !Camera.main.GetComponent<Cinemachine.CinemachineBrain>().IsBlending)
			{
				nextDayEvent?.Raise();
				done = true;
			}
		}
	}
}
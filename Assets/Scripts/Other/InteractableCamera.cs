using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
using Ivyyy.GameEvent;
using Cinemachine;

public class InteractableCamera : MonoBehaviour, InteractableObject
{
	[Header ("Lara Values")]
	[SerializeField] CinemachineVirtualCamera virtualCamera;
	[SerializeField] GameEvent lockPlayer;
	[SerializeField] GameEvent releasePlayer;
	[SerializeField] int priority = 2;

	public void Interact()
	{
		if (virtualCamera != null)
		{
			if (virtualCamera.Priority == 0)
			{
				virtualCamera.Priority = 2;
				lockPlayer?.Raise();
			}
			else
				ReleasePlayer();
		}
	}

	//Private
	private void Start()
	{
		if (virtualCamera == null)
			Debug.LogError ("Missing Virtual Camera!");
		else
			virtualCamera.Priority = 0;

		if (releasePlayer == null || lockPlayer == null)
			Debug.LogError("Missing GameEvents!");
	}

	private void Update()
	{
		if (virtualCamera != null && virtualCamera.Priority > 0 && Input.GetKeyDown (KeyCode.F))
			ReleasePlayer();
	}

	void ReleasePlayer()
	{
		virtualCamera.Priority = 0;
		releasePlayer?.Raise();
	}
}

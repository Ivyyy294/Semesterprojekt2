using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ChatObjContainer
{
	public Button buttonObj;
	public Chat chatObj;
	public bool available;
}

public class PuckTerminal : MonoBehaviour
{
	[SerializeField] Ivyyy.GameEvent.GameEvent closeEvent;
	[SerializeField] Ivyyy.GameEvent.GameEvent settingsEvent;
	
	public ChatObjContainer[] chatObjContainers;
	int currentIndex = -1;

	public void OnSettings()
	{
		settingsEvent?.Raise();
	}

	public void OnClose()
	{
		closeEvent.Raise();
	}

	public void SetActiveChat (int index)
	{
		if (index >= 0 && index < chatObjContainers.Length)
		{
			if (currentIndex != index && chatObjContainers[index].available)
			{
				if (currentIndex != -1)
					chatObjContainers[currentIndex].chatObj.gameObject.SetActive (false);
				
				chatObjContainers[index].chatObj.gameObject.SetActive (true);
				currentIndex = index;
			}
		}
		else
			Debug.LogError ("Invalid Error!");
	}

	public void UnlockChat (int index)
	{
		if (index >= 0 && index < chatObjContainers.Length)
		{
			chatObjContainers[index].buttonObj.gameObject.SetActive(true);
			chatObjContainers[index].available = true;
		}
	}

	//Private Functions
	private void Start()
	{
		foreach (var i in chatObjContainers)
		{
			i.buttonObj.gameObject.SetActive (i.available);
			i.chatObj.gameObject.SetActive (false);
		}

		SetActiveChat (0);
	}

	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}
	private void FixedUpdate()
	{
		Canvas.ForceUpdateCanvases();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PuckTerminal : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] Ivyyy.GameEvent.GameEvent closeEvent;
	[SerializeField] Ivyyy.GameEvent.GameEvent settingsEvent;
	
	[SerializeField] GameObject buttonContainer;
	[SerializeField] GameObject chatContainer;
	[SerializeField] List <ChatButton> buttonObjList;

	int currentIndex = -1;

	//private List <Chat> chatObjList = new List<Chat>();

	//Public Values
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
		if (index >= 0 && index < buttonObjList.Count)
		{
			if (currentIndex != index && buttonObjList[index].available)
			{
				if (currentIndex != -1)
					buttonObjList[currentIndex].ShowChat (false);
				
				buttonObjList[index].ShowChat (true);
				currentIndex = index;
			}
		}
		else
			Debug.LogError ("Invalid Error!");
	}

	public void UnlockChat (int index)
	{
		if (index >= 0 && index < buttonObjList.Count)
			buttonObjList[index].available = true;
	}

	public ChatButton GetChatButtonObj (int index)
	{
		return buttonObjList[index];
	}

	//Private Functions
	private void Start()
	{
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ivyyy.StateMachine;

[System.Serializable]
public class PuckTerminal : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] Ivyyy.GameEvent.GameEvent closeEvent;
	[SerializeField] Ivyyy.GameEvent.GameEvent settingsEvent;
	
	[SerializeField] GameObject loginScreen;

	[SerializeField] GameObject buttonContainer;
	[SerializeField] GameObject chatContainer;
	[SerializeField] List <ChatButton> buttonObjList;
	[SerializeField] List <GameObject> aboutObjList;

	int currentIndex = -1;

	//private List <Chat> chatObjList = new List<Chat>();

	//Public Values
	public void SetPasswordAvailable(bool val)
	{
		PasswordScreen passwordScreen = loginScreen.GetComponentInChildren<PasswordScreen>();

		if (passwordScreen != null)
			passwordScreen.SetPasswordAvailable (val);
	}

	public void ShowAboutPage()
	{
		//Dont show About for Puck
		if (currentIndex != -1 && currentIndex != 3)
		{
			aboutObjList[currentIndex].SetActive(true);
			buttonObjList[currentIndex].ShowChat (false);
		}
	}

	public void ShowAboutPageHelena()
	{
		if (aboutObjList.Count > 0)
		{
			aboutObjList[aboutObjList.Count -1].SetActive(true);

			if (currentIndex != -1)
				buttonObjList[currentIndex].ShowChat (false);
		}
	}

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
			if (buttonObjList[index].available)
			{
				if (currentIndex != -1)
				{
					buttonObjList[currentIndex].IsSelected (false);
					buttonObjList[currentIndex].ShowChat (false);
				}
				
				buttonObjList[index].ShowChat (true);
				buttonObjList[index].IsSelected (true);
				currentIndex = index;
			}
		}
		else
			Debug.LogError ("Invalid Error!");

		HideAboutPages();
	}

	public void SetChatVisible (int index, bool val)
	{
		if (index >= 0 && index < buttonObjList.Count)
			buttonObjList[index].gameObject.SetActive(val);
	}

	public void SetChatAvailable (int index, bool val)
	{
		if (index >= 0 && index < buttonObjList.Count)
			buttonObjList[index].available = val;
	}

	public void UnlockChat (int index)
	{
		SetChatAvailable (index, true);
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

		loginScreen.SetActive (true);

		SetActiveChat (currentIndex);
	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void HideAboutPages()
	{
		foreach (GameObject i in aboutObjList)
		{
			if (i.activeInHierarchy)
				i.SetActive (false);
		}
	}
}

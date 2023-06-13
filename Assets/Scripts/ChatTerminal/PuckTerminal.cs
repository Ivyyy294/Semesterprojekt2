using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ChatObjContainer
{
	public DialogContainer dialog;
	public bool available;
}

public class PuckTerminal : MonoBehaviour
{
	public ChatObjContainer[] dialogList;

	[Header ("Lara Values")]
	[SerializeField] Ivyyy.GameEvent.GameEvent closeEvent;
	[SerializeField] Ivyyy.GameEvent.GameEvent settingsEvent;
	
	[SerializeField] GameObject buttonContainer;
	[SerializeField] GameObject chatContainer;

	int currentIndex = -1;

	private List <Button> buttonObjList = new List<Button>();
	private List <Chat> chatObjList = new List<Chat>();

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
		if (index >= 0 && index < dialogList.Length)
		{
			if (currentIndex != index && dialogList[index].available)
			{
				if (currentIndex != -1)
					chatObjList[currentIndex].gameObject.SetActive (false);
				
				chatObjList[index].gameObject.SetActive (true);
				currentIndex = index;
			}
		}
		else
			Debug.LogError ("Invalid Error!");
	}

	public void UnlockChat (int index)
	{
		if (index >= 0 && index < dialogList.Length)
		{
			buttonObjList[index].gameObject.SetActive(true);
			dialogList[index].available = true;
		}
	}

	public Chat GetChatObj (int index)
	{
		//Making sure chatObjList is valid
		if (chatObjList.Count == 0)
			InitObjLists();

		return chatObjList[index];
	}

	//Private Functions
	private void Start()
	{
		InitObjLists();
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
		//Updating button visiblity
		for (int i = 0; i < dialogList.Length; ++i)
			buttonObjList[i].gameObject.SetActive (dialogList[i].available);
	}

	private void InitObjLists ()
	{
		//ButtonList
		foreach (Button i in buttonContainer.GetComponentsInChildren <Button>())
		{
			i.gameObject.SetActive (false);
			buttonObjList.Add (i);
		}

		//ChatList
		foreach (Chat i in chatContainer.GetComponentsInChildren <Chat>())
		{
			i.gameObject.SetActive (false);
			chatObjList.Add (i);
		}

		for (int i = 0; i < dialogList.Length; ++i)
		{
			int chatNr = i;
			buttonObjList[i].onClick.AddListener (call:() =>{SetActiveChat (chatNr);});
			chatObjList[i].dialogContainer = dialogList[i].dialog;
		}
	}
}

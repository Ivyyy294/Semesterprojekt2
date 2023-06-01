using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecieveNpcMessage : Ivyyy.StateMachine.IState
{
	public string guid;

	DialogNodeData data;
	ChatMessage chatMessage;
	DialogManager manager;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <DialogManager>();
		manager.DisableButtons();

		data = manager.dialogContainer.GetDialogNodeData (guid);

		chatMessage = Object.Instantiate (manager.messageNpcTemplate, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
		chatMessage.SetContent (data);
	}

	public void Update (GameObject obj)
	{
		if (chatMessage.Done)
		{
			if (data.Type == DialogNodeData.NodeType.MultipleChoice)
				manager.SetState (new GetPlayerChoice(){guid = data.Guid});
			else if (data.Type == DialogNodeData.NodeType.Auto)
			{
				var portList = manager.dialogContainer.GetDialogPorts (guid);

				if (portList.Count > 0)
					manager.SetState (new RecieveNpcMessage(){guid = portList[0].targetNodeGuid});
			}
		}
	}
}

public class GetPlayerChoice : Ivyyy.StateMachine.IState
{
	public string guid;

	DialogManager manager;
	private List <NodeLinkData> portList;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <DialogManager>();
		portList = manager.dialogContainer.GetDialogPorts (guid);
		InitButtons ();
	}

	public void Update (GameObject obj) {}

	private void InitButtons()
	{
		for (int i = 0; i < portList.Count; ++i)
		{
			NodeLinkData port = portList[i];
			manager.ButtonList[i].gameObject.SetActive(true);
			manager.ButtonList[i].GetComponent<Button>().onClick.AddListener (call:() =>{ButtonCallBack(port);});
			manager.ButtonList[i].GetComponentInChildren<TextMeshProUGUI>().text = port.portName;
		}
	}

	private void ButtonCallBack (NodeLinkData _port)
	{
		manager.SetState (new SentPlayerMessage() {port = _port});
	}
}

public class SentPlayerMessage : Ivyyy.StateMachine.IState
{
	public NodeLinkData port;
	DialogManager manager;
	ChatMessage chatMessage;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <DialogManager>();
		manager.DisableButtons();

		chatMessage = Object.Instantiate (manager.messagePlayerTemplate, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
		chatMessage.SetContent (port.portName);
	}

	public void Update (GameObject obj)
	{
		if (chatMessage.Done)
			manager.SetState (new RecieveNpcMessage(){guid = port.targetNodeGuid});
	}
}

public class DialogManager : MonoBehaviour
{
	//Get
	public List <GameObject> ButtonList => buttonList;


	[Header ("Lara values")]
	public DialogContainer dialogContainer;
	public GameObject messageContainer;
	public GameObject messageNpcTemplate;
	public GameObject messagePlayerTemplate;
	public GameObject buttonContainer;

	private Ivyyy.StateMachine.IState currentState;
	private List <GameObject> buttonList = new List<GameObject>();

	public void SetState (Ivyyy.StateMachine.IState newState)
	{
		currentState = newState;
		currentState.Enter(gameObject);
	}

    // Start is called before the first frame update
    void Start()
    {
        if (dialogContainer != null)
		{
			for (int i = 0; i < buttonContainer.transform.childCount; ++i)
				buttonList.Add (buttonContainer.transform.GetChild(i).gameObject);

			SetState (new RecieveNpcMessage() {guid = dialogContainer.GetStartNodeGuid()});
		}
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update (gameObject);
    }

	public void DisableButtons ()
	{
		foreach (var i in buttonList)
		{
			i.gameObject.SetActive (false);
			i.GetComponent <Button>().onClick.RemoveAllListeners();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DefaultState : Ivyyy.StateMachine.IState
{
	public string guid;
	DialogNodeData data;
	ChatTerminalUi manager;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <ChatTerminalUi>();
		data = manager.dialogContainer.GetDialogNodeData (guid);
	}

	public void Update  (GameObject obj)
	{
		if (data.Type == DialogNodeData.NodeType.NPC)
		{
			manager.npcNodeState.guid = guid;
			manager.SetState (manager.npcNodeState);
		}
		else if (data.Type == DialogNodeData.NodeType.CHOICE)
		{
			manager.choiceNodeState.guid = guid;
			manager.SetState (manager.choiceNodeState);
		}
		else if (data.Type == DialogNodeData.NodeType.EVENT)
		{
			manager.eventNodeState.guid = guid;
			manager.SetState (manager.eventNodeState);
		}
	}
}

public class NpcNodeState : Ivyyy.StateMachine.IState
{
	public string guid;

	DialogNodeData data;
	ChatMessage chatMessage;
	ChatTerminalUi manager;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <ChatTerminalUi>();
		manager.DisableButtons();

		data = manager.dialogContainer.GetDialogNodeData (guid);

		chatMessage = Object.Instantiate (manager.messageNpcTemplate, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
		chatMessage.SetContent (data);
	}

	public void Update (GameObject obj)
	{
		if (chatMessage.Done)
		{
			var portList = manager.dialogContainer.GetDialogPorts (guid);

			if (portList.Count > 0)
			{
				manager.defaultState.guid = portList[0].targetNodeGuid;
				manager.SetState (manager.defaultState);
			}
		}
	}
}

public class ChoiceNodeState : Ivyyy.StateMachine.IState
{
	public string guid;

	ChatTerminalUi manager;
	private List <NodeLinkData> portList;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <ChatTerminalUi>();
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
		manager.playerMessageState.port = _port;
		manager.SetState (manager.playerMessageState);
	}
}

public class PlayerMessageState : Ivyyy.StateMachine.IState
{
	public NodeLinkData port;
	ChatTerminalUi manager;
	ChatMessage chatMessage;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <ChatTerminalUi>();
		manager.DisableButtons();

		chatMessage = Object.Instantiate (manager.messagePlayerTemplate, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
		chatMessage.SetContent (port.portName);
	}

	public void Update (GameObject obj)
	{
		if (chatMessage.Done)
		{
			manager.defaultState.guid = port.targetNodeGuid;
			manager.SetState (manager.defaultState);
		}
	}
}

public class EventNodeState : Ivyyy.StateMachine.IState
{
	public string guid;
	DialogNodeData data;
	ChatTerminalUi manager;

	public void Enter (GameObject obj)
	{
		manager = obj.GetComponent <ChatTerminalUi>();
		data = manager.dialogContainer.GetDialogNodeData (guid);
		data.GameEvent.Raise();
	}

	public void Update  (GameObject obj)
	{
		var portList = manager.dialogContainer.GetDialogPorts (guid);

		if (portList.Count > 0)
		{
			manager.defaultState.guid = portList[0].targetNodeGuid;
			manager.SetState (manager.defaultState);
		}
	}
}


public class ChatTerminalUi : MonoBehaviour
{
	//Get
	public List <GameObject> ButtonList => buttonList;

	[Header ("Lara values")]
	public DialogContainer dialogContainer;
	public GameObject messageContainer;
	public GameObject messageNpcTemplate;
	public GameObject messagePlayerTemplate;
	public GameObject buttonContainer;
	public Ivyyy.GameEvent.GameEvent closeEvent;

	private Ivyyy.StateMachine.IState currentState;
	private List <GameObject> buttonList = new List<GameObject>();

	public DefaultState defaultState = new DefaultState();
	public NpcNodeState npcNodeState = new NpcNodeState();
	public ChoiceNodeState choiceNodeState = new ChoiceNodeState();
	public PlayerMessageState playerMessageState = new PlayerMessageState();
	public EventNodeState eventNodeState = new EventNodeState();

	//Public Functions
	public void Show(bool val)
	{
		gameObject.SetActive (val);
	}

	public void RaiseCloseEvent()
	{
		closeEvent.Raise();
	}

	public void SetState (Ivyyy.StateMachine.IState newState)
	{
		currentState = newState;
		currentState.Enter(gameObject);
	}

	public void DisableButtons ()
	{
		foreach (var i in buttonList)
		{
			i.gameObject.SetActive (false);
			i.GetComponent <Button>().onClick.RemoveAllListeners();
		}
	}

	//Private Functions
	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Start()
    {
        if (dialogContainer != null)
		{
			for (int i = 0; i < buttonContainer.transform.childCount; ++i)
				buttonList.Add (buttonContainer.transform.GetChild(i).gameObject);

			defaultState.guid = dialogContainer.GetStartNodeGuid();
			SetState (defaultState);
		}
    }

    void Update()
    {
        currentState.Update (gameObject);
    }

}

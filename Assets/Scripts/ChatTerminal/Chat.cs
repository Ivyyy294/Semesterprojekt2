using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ivyyy.SaveGameSystem;

public abstract class BaseState: Ivyyy.StateMachine.IState
{
	protected DialogTree.Node node;
	protected Chat manager;

	public virtual void Enter (GameObject obj)
	{
		manager = obj.GetComponent <Chat>();
		node = manager.DialogTree.CurrentNode();
	}

	public abstract void Update  (GameObject obj);
}

public class DefaultState : BaseState
{
	public override void Update  (GameObject obj)
	{
		if (node.data == null)
			return;
		else if (node.data.Type == DialogNodeData.NodeType.NPC)
			manager.SetState (manager.npcNodeState);
		else if (node.data.Type == DialogNodeData.NodeType.CHOICE)
			manager.SetState (manager.choiceNodeState);
		else if (node.data.Type == DialogNodeData.NodeType.RAISE_EVENT)
			manager.SetState (manager.raiseEventNodeState);
		else if (node.data.Type == DialogNodeData.NodeType.LISTEN_EVENT)
			manager.SetState (manager.listenEventNodeState);
		else if (node.data.Type == DialogNodeData.NodeType.LOGIC)
			manager.SetState (manager.logicNodeState);
		else if (node.data.Type == DialogNodeData.NodeType.WAIT)
			manager.SetState (manager.waitNodeState);
	}
}

public class NpcNodeState : BaseState
{
	ChatMessage chatMessage;

	public override void Enter (GameObject obj)
	{
		base.Enter (obj);

		chatMessage = Object.Instantiate (manager.messageNpcTemplate, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
		chatMessage.SetContent (node.data);
		Canvas.ForceUpdateCanvases();
	}

	public override void Update (GameObject obj)
	{
		if (chatMessage.Done)
		{
			if (node.ports.Count > 0)
			{
				manager.DialogTree.Next();
				manager.SetState (manager.defaultState);
			}
		}
	}
}

public class ChoiceNodeState : BaseState
{
	ChatMessage chatMessage;
	int portSelected;

	public override void Enter (GameObject obj)
	{
		base.Enter (obj);

		chatMessage = null;
		portSelected = -1;

		InitButtons ();
	}

	public override void Update (GameObject obj)
	{
		if (portSelected != -1 && chatMessage != null && chatMessage.Done)
		{
			manager.DialogTree.Next (portSelected);
			manager.SetState(manager.defaultState);
		}
	}

	public void InitButtons ()
	{
		for (int i = 0; i < node.ports.Count; ++i)
		{
			int portNr = i;
			NodeLinkData port = node.ports[i];
			manager.ButtonList[i].gameObject.SetActive(true);
			manager.ButtonList[i].GetComponent<Button>().onClick.AddListener (call:() =>{ButtonCallBack(portNr);});
			manager.ButtonList[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString() + ") " + port.portName;
		}
	}

	private void ButtonCallBack (int port)
	{
		manager.DisableButtons();
		portSelected = port;
		chatMessage = Object.Instantiate (manager.messagePlayerTemplate, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
		chatMessage.SetContent (node.ports[port].portName);
	}
}

public class RaiseEventNodeState : BaseState
{
	public override void Enter (GameObject obj)
	{
		base.Enter (obj);
		node.data.GameEvent?.Raise();
	}

	public override void Update  (GameObject obj)
	{
		manager.DialogTree.Next();
		manager.SetState (manager.defaultState);
	}
}

//ToDo it should be able to jump into every ListenEventNodeState at any time
public class ListenEventNodeState : BaseState, Ivyyy.GameEvent.IGameEventListener
{
	bool done;

	public override void Enter (GameObject obj)
	{
		base.Enter (obj);
		done = false;
		node.data.GameEvent.RegisterListener (this);
	}

	public override void Update  (GameObject obj)
	{
		if (done)
		{
			node.data.GameEvent.UnregisterListener (this);
			manager.DialogTree.Next();
			manager.SetState (manager.defaultState);
		}
	}

	public void OnEventRaised()
	{
		done = true;
	}
}

public class LogicNodeState : BaseState
{
	public override void Update  (GameObject obj)
	{
		BlackBoardProperty checkValue = node.data.BlackBoardProperty;
		BlackBoardProperty property = BlackBoard.Me().GetProperty (checkValue.guid);

		if (property.Compare (checkValue))
			True();
		else
			False();
	}

	private void True()
	{
		manager.DialogTree.Next(0);
		manager.SetState (manager.defaultState);
	}

	private void False()
	{
		manager.DialogTree.Next(1);
		manager.SetState (manager.defaultState);
	}
}

public class WaitNodeState : BaseState
{
	public override void Update  (GameObject obj)
	{
		BlackBoardProperty checkValue = node.data.BlackBoardProperty;
		BlackBoardProperty property = BlackBoard.Me().GetProperty (checkValue.guid);

		if (property.Compare (checkValue))
		{
			manager.DialogTree.Next(0);
			manager.SetState (manager.defaultState);
		}
	}
}

public class Chat : MonoBehaviour
{
	public DialogContainer dialogContainer;

	[Header ("Lara values")]
	public GameObject messageContainer;
	public GameObject messageNpcTemplate;
	public GameObject messagePlayerTemplate;

	[SerializeField] GameObject buttonContainer;

	public DefaultState defaultState = new DefaultState();
	public NpcNodeState npcNodeState = new NpcNodeState();
	public ChoiceNodeState choiceNodeState = new ChoiceNodeState();
	public RaiseEventNodeState raiseEventNodeState = new RaiseEventNodeState();
	public ListenEventNodeState listenEventNodeState = new ListenEventNodeState();
	public LogicNodeState logicNodeState = new LogicNodeState();
	public WaitNodeState waitNodeState = new WaitNodeState();

	private Ivyyy.StateMachine.IState currentState;
	
	private DialogTree dialogTree = new DialogTree();
	public DialogTree DialogTree => dialogTree;

	private List <GameObject> buttonList = new List<GameObject>();
	public List <GameObject> ButtonList => buttonList;

	//Public Functions
	public Payload GetPayload (string nodeName)
	{
		Payload chatPayload = new Payload(nodeName);

		//Removes the empty end Node
		if (DialogTree.nodesVisited.Count > 0 && DialogTree.nodesVisited.Peek().data == null)
			DialogTree.nodesVisited.Pop();

		DialogTree.Node[] nodeArray = DialogTree.nodesVisited.ToArray();
		int nodeCount = nodeArray.Length;
		chatPayload.Add ("Count", nodeCount);

		//Save items in reverse order
		for (int j = 0; j < nodeCount; ++j)
		{
			int index = nodeCount -1 - j;
			DialogTree.Node node = DialogTree.nodesVisited.ToArray()[index];
			chatPayload.Add ("Chat" + j, node.data.Guid);
		}

		return chatPayload;
	}

	public void LoadObject (Payload val)
	{
		int nodeCount = int.Parse (val.data["Count"]);

		DialogTree.nodesVisited.Clear();

		List <string> nodeList = new List<string>();

		for (int j = 0; j < nodeCount; ++j)
			nodeList.Add (val.data["Chat" + j.ToString()]);

		LoadSaveGame (nodeList);
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
	void Start()
    {
		Init();

		if (dialogTree.nodesVisited.Count == 0)
			dialogTree.Next();

		SetState (defaultState);
    }

    void Update()
    {
        currentState.Update (gameObject);
    }

	void OnEnable()
	{
		if (buttonList.Count == 0)
			InitButtonList();
		//Reinitilize keys
		DisableButtons();

		if (currentState == choiceNodeState)
			choiceNodeState.InitButtons();
	}

	private void Init()
	{
		InitButtonList();

        if (dialogContainer != null)
			dialogTree.dialogContainer = dialogContainer;
	}

	void InitButtonList()
	{
		buttonList.Clear();

		for (int i = 0; i < buttonContainer.transform.childCount; ++i)
			buttonList.Add (buttonContainer.transform.GetChild(i).gameObject);
	}

	void LoadSaveGame (List <string> nodeList)
	{
		Init();
		dialogTree.nodesVisited.Clear();

		for (int i = 0; i < nodeList.Count; ++i)
		{
			dialogTree.Next (nodeList[i]);
			DialogTree.Node node = dialogTree.CurrentNode();

			if (node.data.Type == DialogNodeData.NodeType.NPC)
			{
				ChatMessage chatMessage = Object.Instantiate (messageNpcTemplate, messageContainer.transform).GetComponentInChildren<ChatMessage>();
				chatMessage.SetContent (node.data, true);
			}

			else if (node.data.Type == DialogNodeData.NodeType.CHOICE)
			{
				int nextIndex = i + 1;

				if (nextIndex < nodeList.Count)
				{
					string nextGuid = dialogTree.dialogContainer.GetDialogNodeData (nodeList[nextIndex]).Guid;

					ChatMessage chatMessage = Object.Instantiate (messagePlayerTemplate, messageContainer.transform).GetComponentInChildren<ChatMessage>();

					foreach (var port in node.ports)
					{
						if (nextGuid == port.targetNodeGuid)
							chatMessage.SetContent (port.portName, true);
					}
				}
			}
		}

		//If the last node is an NPC node, move to the next Node to avoid duplicated messages
		if (dialogTree.CurrentNode().data != null && dialogTree.CurrentNode().data.Type == DialogNodeData.NodeType.NPC)
			dialogTree.Next();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class BaseState: Ivyyy.StateMachine.IState
{
	protected DialogTree.Node node;
	protected ChatTerminalUi manager;

	public virtual void Enter (GameObject obj)
	{
		manager = obj.GetComponent <ChatTerminalUi>();
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

		//manager.DisableButtons();
		chatMessage = Object.Instantiate (manager.messageNpcTemplate, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
		chatMessage.SetContent (node.data);
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

	private void InitButtons ()
	{
		for (int i = 0; i < node.ports.Count; ++i)
		{
			int portNr = i;
			NodeLinkData port = node.ports[i];
			manager.ButtonList[i].gameObject.SetActive(true);
			manager.ButtonList[i].GetComponent<Button>().onClick.AddListener (call:() =>{ButtonCallBack(portNr);});
			manager.ButtonList[i].GetComponentInChildren<TextMeshProUGUI>().text = port.portName;
		}
	}

	private void DisableButtons ()
	{
		foreach (var i in manager.ButtonList)
		{
			i.gameObject.SetActive (false);
			i.GetComponent <Button>().onClick.RemoveAllListeners();
		}
	}

	private void ButtonCallBack (int port)
	{
		DisableButtons();
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

public class ChatTerminalUi : MonoBehaviour
{
	[SerializeField] DialogContainer dialogContainer;

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
	public void SetState (Ivyyy.StateMachine.IState newState)
	{
		currentState = newState;
		currentState.Enter(gameObject);
	}

	//Private Functions
	void Start()
    {
		buttonList.Clear();

		for (int i = 0; i < buttonContainer.transform.childCount; ++i)
			buttonList.Add (buttonContainer.transform.GetChild(i).gameObject);

        if (dialogContainer != null)
		{
			dialogTree.dialogContainer = dialogContainer;
			dialogTree.Next();
			SetState (defaultState);
		}
    }

    void Update()
    {
        currentState.Update (gameObject);
    }

	internal void SetState(object waitNodeState)
	{
		throw new System.NotImplementedException();
	}
}

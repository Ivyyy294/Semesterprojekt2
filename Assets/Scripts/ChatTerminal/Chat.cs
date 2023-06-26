using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ivyyy.SaveGameSystem;
using Ivyyy.StateMachine;

public class Chat : FiniteStateMachine
{
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

		public virtual void Exit(GameObject obj) {}
	}

	public class PuckState : BaseState
	{
		public override void Update  (GameObject obj)
		{
			node.data.audioAsset?.Play();
			Puck.SetEmotion (node.data.emotion);
			manager.DialogTree.Next();
			manager.EnterState (manager.defaultState);
		}
	}

	public class DefaultState : BaseState
	{
		public override void Update  (GameObject obj)
		{
			if (node.data == null)
				return;
			else if (node.data.Type == DialogNodeData.NodeType.NPC)
				manager.EnterState (manager.npcNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.CHOICE)
				manager.EnterState (manager.choiceNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.RAISE_EVENT)
				manager.EnterState (manager.raiseEventNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.LISTEN_EVENT)
				manager.EnterState (manager.listenEventNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.LOGIC)
				manager.EnterState (manager.logicNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.WAIT)
				manager.EnterState (manager.waitNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.PUCK)
				manager.EnterState (manager.puckState);
			else if (node.data.Type == DialogNodeData.NodeType.EDIT_VALUE)
				manager.EnterState (manager.editValueNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.PLAYER_AUTO)
				manager.EnterState (manager.playerAutoNodeState);
			else if (node.data.Type == DialogNodeData.NodeType.START)
			{
				manager.DialogTree.Next();
				node = manager.DialogTree.CurrentNode();
			}
		}
	}

	[System.Serializable]
	public class NpcNodeState : BaseState
	{
		public GameObject msgPrefab;
		ChatMessage chatMessage;
		float timer;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			timer = 0f;
			chatMessage = Object.Instantiate (msgPrefab, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
			chatMessage.SetContent (node.data);
			Canvas.ForceUpdateCanvases();
		}

		public override void Update (GameObject obj)
		{
			if (chatMessage.Done)
			{
				DialogTree.Node nextNode = manager.DialogTree.Peek();

				bool wait = nextNode.data != null
					&& (nextNode.data.Type == DialogNodeData.NodeType.NPC
					|| nextNode.data.Type == DialogNodeData.NodeType.PLAYER_AUTO);

				if (wait && timer < manager.delayNpcMessage)
					timer += Time.deltaTime;
				else if (node.ports.Count > 0)
				{
					manager.DialogTree.Next();
					manager.EnterState (manager.defaultState);
				}
			}
		}
	}

	[System.Serializable]
	public class ChoiceNodeState : BaseState
	{
		public GameObject msgPrefab;
		ChatMessage chatMessage;
		int portSelected;
		float timer = 0f;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);

			timer = 0f;
			chatMessage = null;
			portSelected = -1;

			InitButtons ();
		}

		public override void Update (GameObject obj)
		{
			if (portSelected != -1 && chatMessage != null && chatMessage.Done)
			{
				if (timer <= manager.delayPlayerMessage)
					timer += Time.deltaTime;
				else
				{
					manager.DialogTree.Next (portSelected);
					manager.EnterState(manager.defaultState);
				}
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
			chatMessage = Object.Instantiate (msgPrefab, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
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
			manager.EnterState (manager.defaultState);
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
				manager.EnterState (manager.defaultState);
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
			manager.EnterState (manager.defaultState);
		}

		private void False()
		{
			manager.DialogTree.Next(1);
			manager.EnterState (manager.defaultState);
		}
	}

	public class WaitNodeState : BaseState
	{
		public override void Update  (GameObject obj)
		{
			BlackBoardProperty checkValue = node.data.BlackBoardProperty;
			BlackBoardProperty property = BlackBoard.Me().GetProperty (checkValue.guid);

			if (property != null && property.Compare (checkValue))
			{
				manager.DialogTree.Next(0);
				manager.EnterState (manager.defaultState);
			}
		}
	}

	public class EditValueNodeState : BaseState
	{
		public override void Update  (GameObject obj)
		{
			BlackBoardProperty checkValue = node.data.BlackBoardProperty;
			BlackBoard.Me().EditValue (checkValue.guid, node.data.editTyp, checkValue.iVal);
			manager.DialogTree.Next(0);
			manager.EnterState (manager.defaultState);
		}
	}

	[System.Serializable]
	public class PlayerAutoNodeState : BaseState
	{
		public GameObject msgPrefab;
		ChatMessage chatMessage;
		float timer;

		public override void Enter (GameObject obj)
		{
			base.Enter (obj);
			timer = 0f;
			chatMessage = Object.Instantiate (msgPrefab, manager.messageContainer.transform).GetComponentInChildren<ChatMessage>();
			chatMessage.SetContent (node.data);
			Canvas.ForceUpdateCanvases();
		}

		public override void Update (GameObject obj)
		{
			if (chatMessage.Done)
			{
				DialogTree.Node nextNode = manager.DialogTree.Peek();

				bool wait = nextNode.data != null
					&& (nextNode.data.Type == DialogNodeData.NodeType.NPC
					|| nextNode.data.Type == DialogNodeData.NodeType.PLAYER_AUTO);

				if (wait && timer < manager.delayPlayerMessage)
					timer += Time.deltaTime;
				else if (node.ports.Count > 0)
				{
					manager.DialogTree.Next();
					manager.EnterState (manager.defaultState);
				}
			}
		}
	}

	public DialogContainer dialogContainer;
	public float delayPlayerMessage = 0.5f;
	public float delayNpcMessage = 0.5f;

	[Header ("Lara values")]
	public GameObject messageContainer;
	[SerializeField] GameObject buttonContainer;

	public DefaultState defaultState = new DefaultState();
	public NpcNodeState npcNodeState = new NpcNodeState();
	public ChoiceNodeState choiceNodeState = new ChoiceNodeState();
	public RaiseEventNodeState raiseEventNodeState = new RaiseEventNodeState();
	public ListenEventNodeState listenEventNodeState = new ListenEventNodeState();
	public LogicNodeState logicNodeState = new LogicNodeState();
	public WaitNodeState waitNodeState = new WaitNodeState();
	public PuckState puckState = new PuckState();
	public EditValueNodeState editValueNodeState = new EditValueNodeState();
	public PlayerAutoNodeState playerAutoNodeState = new PlayerAutoNodeState();
	
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

		EnterState (defaultState);
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
				ChatMessage chatMessage = Object.Instantiate (npcNodeState.msgPrefab, messageContainer.transform).GetComponentInChildren<ChatMessage>();
				chatMessage.SetContent (node.data, true);
			}
			else if (node.data.Type == DialogNodeData.NodeType.PLAYER_AUTO)
			{
				ChatMessage chatMessage = Object.Instantiate (playerAutoNodeState.msgPrefab, messageContainer.transform).GetComponentInChildren<ChatMessage>();
				chatMessage.SetContent (node.data, true);
			}
			else if (node.data.Type == DialogNodeData.NodeType.CHOICE)
			{
				int nextIndex = i + 1;

				if (nextIndex < nodeList.Count)
				{
					string nextGuid = dialogTree.dialogContainer.GetDialogNodeData (nodeList[nextIndex]).Guid;

					ChatMessage chatMessage = Object.Instantiate (choiceNodeState.msgPrefab, messageContainer.transform).GetComponentInChildren<ChatMessage>();

					foreach (var port in node.ports)
					{
						if (nextGuid == port.targetNodeGuid)
							chatMessage.SetContent (port.portName, true);
					}
				}
			}
		}

		//If the last node is an NPC node, move to the next Node to avoid duplicated messages
		if (dialogTree.CurrentNode().data != null && (dialogTree.CurrentNode().data.Type == DialogNodeData.NodeType.NPC
			|| dialogTree.CurrentNode().data.Type == DialogNodeData.NodeType.PLAYER_AUTO
			|| dialogTree.CurrentNode().data.Type == DialogNodeData.NodeType.RAISE_EVENT))
			dialogTree.Next();
	}
}

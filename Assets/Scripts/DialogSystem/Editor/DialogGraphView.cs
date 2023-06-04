using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

public class DialogGraphView : GraphView
{
	public DialogGraphView()
	{
		SetupZoom (ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

		this.AddManipulator (new ContentDragger());
		this.AddManipulator (new SelectionDragger());
		this.AddManipulator (new RectangleSelector());
		this.AddManipulator (CreateNodeContextMenu());
		this.AddManipulator (CreateTextNodeContextMenu());
		this.AddManipulator (CreateRaiseEventNodeContextMenu());
		this.AddManipulator (CreateListenEventNodeContextMenu());
		this.AddManipulator (CreateGroupContextMenu());

		////ToDo fix stylesheet
		//var grid = new GridBackground();
		//Insert (0, grid);
		//grid.StretchToParentSize();

		AddElement (GenerateEntryPointNode());
		AddStyles();
	}

	private void AddStyles()
	{
		StyleSheet nodeStyleSheet = (StyleSheet) EditorGUIUtility.Load ("DSNodeStyles.uss");
		styleSheets.Add (nodeStyleSheet);
	}

	private IManipulator CreateListenEventNodeContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add ListenEventNode", actionEvent=>AddElement (DialogListenEventNode.Create("ListenEventNode", actionEvent.eventInfo.localMousePosition))));

		return contextualMenuManipulator;
	}

	private IManipulator CreateTextNodeContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add NpcNode", actionEvent=>AddElement (DialogNpcNode.CreateNode("NpcNode", actionEvent.eventInfo.localMousePosition, this))));

		return contextualMenuManipulator;
	}

	//Context Manipulator
	private IManipulator CreateRaiseEventNodeContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add RaiseEventNode", actionEvent=>AddElement (DialogRaiseEventNode.Create("RaiseEventNode", actionEvent.eventInfo.localMousePosition))));

		return contextualMenuManipulator;
	}

	private IManipulator CreateNodeContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add ChoiceNode", actionEvent=>AddElement (CreateChoiceNode("ChoiceNode", actionEvent.eventInfo.localMousePosition))));

		return contextualMenuManipulator;
	}

	private IManipulator CreateGroupContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add Group", actionEvent=>CreateGroup("DialogGroup", actionEvent.eventInfo.localMousePosition)));

		return contextualMenuManipulator;
	}

	public Group CreateGroup (string title, Vector2 localMousePosition)
	{
		Group group = DialogGraphUtility.CreateGroup (title, localMousePosition);
		AddElement (group);

		foreach (GraphElement selectedElement in selection)
		{
			if (!(selectedElement is DialogNode)) continue;

			DialogNode dialogNode = (DialogNode) selectedElement;
			group.AddElement (dialogNode);
		}

		return group;
	}

	//public void CreateChoiceNode (string nodeName) 
	//{
	//	AddElement (CreateChoiceNode (nodeName));
	//}

	public DialogNode CreateChoiceNode (string nodeName, Vector2 pos) 
	{
		DialogNode node = CreateChoiceNode (nodeName);
		node.SetPosition (new Rect (pos, DialogNode.defaultSize));

		return node;
	}

	public DialogNode CreateChoiceNode (string nodeName)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			DialogText = "hello world",
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.CHOICE
		};

		return CreateChoiceNode (data);
	}

	public DialogNode CreateChoiceNode (DialogNodeData nodeData)
	{
		var dialogNode = DialogGraphUtility.CreateChoiceNode (nodeData);

		dialogNode.Init(this);

		return dialogNode;
	}

	private DialogNode GenerateEntryPointNode()
	{ 
		var node = new DialogNode
		{
			entryPoint = true,
			title = "START",

			data = new DialogNodeData
			{
				DialogTitle = "START",
				Guid = System.Guid.NewGuid().ToString(),
				DialogText = "ENTRYPOINT",
			}
		};

		node.Init (this);

		node.RefreshExpandedState();
		node.RefreshPorts();

		node.SetPosition (new Rect (100, 200, 100, 150));

		return node;
	}

	public void RemovePort(DialogNode dialogNode, Port generatedPort)
	{
		var targetEdge = edges.ToList().Where (x=>x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

		//remove edges first
		if (targetEdge.Any())
		{
			var edge = targetEdge.First();
			edge.input.Disconnect (edge);

			RemoveElement (targetEdge.First());
		}

		dialogNode.outputContainer.Remove (generatedPort);
		dialogNode.RefreshPorts();
		dialogNode.RefreshExpandedState();
	}

	public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
	{
		var compatiblePorts = new List <Port>();

		ports.ForEach (port =>
		{
			if (startPort != port && startPort.node != port.node)
				compatiblePorts.Add (port);
		});

		return compatiblePorts;
	}
}

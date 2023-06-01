using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Linq;

public class DialogGraphView : GraphView
{
	//public readonly Vector2 defaultSize = new Vector2 (150, 200);

	public DialogGraphView()
	{
		SetupZoom (ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

		this.AddManipulator (new ContentDragger());
		this.AddManipulator (new SelectionDragger());
		this.AddManipulator (new RectangleSelector());
		this.AddManipulator (CreateNodeContextMenu());
		this.AddManipulator (CreateEventNodeContextMenu());
		this.AddManipulator (CreateGroupContextMenu());

		////ToDo fix stylesheet
		//var grid = new GridBackground();
		//Insert (0, grid);
		//grid.StretchToParentSize();

		AddElement (GenerateEntryPointNode());
	}

	//Context Manipulator
	private IManipulator CreateEventNodeContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add EventNode", actionEvent=>AddElement (CreateEventNode("Event Node", actionEvent.eventInfo.localMousePosition))));

		return contextualMenuManipulator;
	}

	private IManipulator CreateNodeContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add Node", actionEvent=>AddElement (CreateNode("Dialog Node", actionEvent.eventInfo.localMousePosition))));

		return contextualMenuManipulator;
	}

	private IManipulator CreateGroupContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			menuEvent=> menuEvent.menu.AppendAction ("Add Group", actionEvent=>CreateGroup("DialogGroup", actionEvent.eventInfo.localMousePosition)));

		return contextualMenuManipulator;
	}

	//Create Element Functions
	public DialogNode CreateEventNode(string nodeName)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			DialogText = "hello world",
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.GameEvent
		};

		return CreateEventNode (data);
	}

	public GraphElement CreateEventNode(string nodeName, Vector2 localMousePosition)
	{
		DialogNode node = CreateEventNode (nodeName);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}

	public DialogNode CreateEventNode(DialogNodeData data)
	{
		DialogEventNode node = DialogGraphUtility.CreateEventNode (data);
		node.Init (this);
		return node;
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

	public void CreateNode (string nodeName) 
	{
		AddElement (CreateDialogNode (nodeName));
	}

	public DialogNode CreateNode (string nodeName, Vector2 pos) 
	{
		DialogNode node = CreateDialogNode (nodeName);
		node.SetPosition (new Rect (pos, DialogNode.defaultSize));

		return node;
	}

	public DialogNode CreateDialogNode (string nodeName)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			DialogText = "hello world",
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.MultipleChoice
		};

		return CreateDialogNode (data);
	}

	public DialogNode CreateDialogNode (DialogNodeData nodeData)
	{
		var dialogNode = DialogGraphUtility.CreateDialogNode (nodeData);

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

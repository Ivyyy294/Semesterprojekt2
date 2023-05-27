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
		this.AddManipulator (CreateGroupContextMenu());

		//ToDo fix stylesheet
		var grid = new GridBackground();
		Insert (0, grid);
		grid.StretchToParentSize();

		AddElement (GenerateEntryPointNode());
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
			menuEvent=> menuEvent.menu.AppendAction ("Add Group", actionEvent=>AddElement (CreateGroup("DialogGroup", actionEvent.eventInfo.localMousePosition))));

		return contextualMenuManipulator;
	}

	private Group CreateGroup(string title, Vector2 localMousePosition)
	{
		Group group = new Group {title = title};
		group.SetPosition (new Rect (localMousePosition, Vector2.zero));
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
			Guid =  System.Guid.NewGuid().ToString()
		};

		return CreateDialogNode (data);
	}

	public DialogNode CreateDialogNode (DialogNodeData nodeData)
	{
		var dialogNode = new DialogNode
		{
			title = nodeData.DialogTitle,
			dialogText = nodeData.DialogText,
			GUID = nodeData.Guid
		};

		dialogNode.Init();

		//Button
		dialogNode.titleContainer.Add (DialogGraphUtility.CreateButton ("New Choice", onClick:() => {AddChoicePort (dialogNode); }));

		return dialogNode;
	}


	public void AddChoicePort(DialogNode dialogNode, string overriddenPortName = "")
	{
		var generatedPort = DialogGraphUtility.CreatePort (dialogNode, Direction.Output);

		//Remove default name label
		var oldLabel = generatedPort.contentContainer.Q<Label>("type");
		generatedPort.contentContainer.Remove (oldLabel);

		var outputPortCount = dialogNode.outputContainer.Query ("connector").ToList().Count;

		string choicePortName = string.IsNullOrEmpty (overriddenPortName)
			? $"Choice {outputPortCount}"
			: overriddenPortName;
		
		//Invisible Label to enable port drag and drop
		generatedPort.contentContainer.Add (new Label ("\t\t"));
		var textField = DialogGraphUtility.CreateTextField (choicePortName, evt => generatedPort.portName = evt.newValue);
		generatedPort.contentContainer.Add (textField);

		//Button
		generatedPort.contentContainer.Add (DialogGraphUtility.CreateButton ("X", onClick:() =>RemovePort (dialogNode, generatedPort)));

		generatedPort.portName = choicePortName;

		dialogNode.outputContainer.Add (generatedPort);
		dialogNode.RefreshPorts();
		dialogNode.RefreshExpandedState();		
	}

	private void RemovePort(DialogNode dialogNode, Port generatedPort)
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

	private DialogNode GenerateEntryPointNode()
	{ 
		var node = new DialogNode
		{
			title = "START",
			GUID = System.Guid.NewGuid().ToString(),
			dialogText = "ENTRYPOINT",
			entryPoint = true
		};

		var generatedPort = DialogGraphUtility.CreatePort (node, Direction.Output);
		generatedPort.portName = "Next";
		node.outputContainer.Add (generatedPort);

		node.RefreshExpandedState();
		node.RefreshPorts();

		node.SetPosition (new Rect (100, 200, 100, 150));

		return node;
	}
}

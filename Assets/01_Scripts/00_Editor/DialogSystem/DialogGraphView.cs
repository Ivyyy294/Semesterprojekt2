using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Linq;

public class DialogGraphView : GraphView
{
	public readonly Vector2 defaultSize = new Vector2 (150, 200);

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
		node.SetPosition (new Rect (pos, defaultSize));

		return node;
	}

	public DialogNode CreateDialogNode (DialogNodeData nodeData)
	{
		var dialogNode = new DialogNode
		{
			//title = nodeData.DialogTitle,
			dialogText = nodeData.DialogText,
			GUID = nodeData.Guid
		};

		InitDialogNode (dialogNode);

		return dialogNode;
	}

	public DialogNode CreateDialogNode (string nodeName)
	{
		var dialogNode = new DialogNode
		{
			title = nodeName,
			dialogText = "hello world",
			GUID = System.Guid.NewGuid().ToString()
		};

		InitDialogNode (dialogNode);

		return dialogNode;
	}

	public void AddChoicePort(DialogNode dialogNode, string overriddenPortName = "")
	{
		var generatedPort = GeneratePort (dialogNode, Direction.Output);

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

		var deleteButton = new Button (clickEvent:() => RemovePort (dialogNode, generatedPort))
		{
			text = "X"
		};
		generatedPort.contentContainer.Add (deleteButton);

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

	private Port GeneratePort (DialogNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
	{
		return node.InstantiatePort (Orientation.Horizontal, portDirection, capacity, typeof (float));
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

		var generatedPort = GeneratePort (node, Direction.Output);
		generatedPort.portName = "Next";
		node.outputContainer.Add (generatedPort);

		node.RefreshExpandedState();
		node.RefreshPorts();

		node.SetPosition (new Rect (100, 200, 100, 150));

		return node;
	}

	private void InitDialogNode (DialogNode dialogNode)
	{
		//Title
		var titleField = DialogGraphUtility.CreateTextField ("Title", dialogNode.title, evt=> {dialogNode.title = evt.newValue;});
		titleField.SetValueWithoutNotify (dialogNode.title);		
		dialogNode.mainContainer.Add (titleField);

		//Create Input Port
		var inputPort = GeneratePort (dialogNode, Direction.Input, Port.Capacity.Multi);
		inputPort.portName = "Input";

		var button = new Button (clickEvent:() => {AddChoicePort (dialogNode);});
		button.text = "New Choice";
		dialogNode.titleContainer.Add (button);


		//Content
		Foldout textFoldout = new Foldout {text = "Dialog Text"};
		textFoldout.Add (DialogGraphUtility.CreateTextArea(dialogNode.dialogText, evt=>{dialogNode.dialogText = evt.newValue;}));

		dialogNode.mainContainer.Add (textFoldout);

		dialogNode.inputContainer.Add (inputPort);
		dialogNode.RefreshExpandedState();
		dialogNode.RefreshPorts();
		dialogNode.SetPosition (new Rect (Vector2.zero, defaultSize));
	}
}

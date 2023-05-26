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

		var grid = new GridBackground();
		Insert (0, grid);
		grid.StretchToParentSize();

		AddElement (GenerateEntryPointNode());
	}

	public void CreateNode (string nodeName) 
	{
		AddElement (CreateDialogNode (nodeName));
	}

	public DialogNode CreateDialogNode (string nodeName)
	{
		var dialogNode = new DialogNode
		{
			title = nodeName,
			dialogText = nodeName,
			GUID = System.Guid.NewGuid().ToString()
		};

		var inputPort = GeneratePort (dialogNode, Direction.Input, Port.Capacity.Multi);
		inputPort.portName = "Input";

		var button = new Button (clickEvent:() => {AddChoicePort (dialogNode);});
		button.text = "New Choice";
		dialogNode.titleContainer.Add (button);

		var textField = new TextField (string.Empty);
		textField.RegisterValueChangedCallback (evt=>
		{
			dialogNode.dialogText = evt.newValue;
			//dialogNode.title = evt.newValue;
		});
		textField.SetValueWithoutNotify (dialogNode.dialogText);
		
		dialogNode.mainContainer.Add (textField);

		dialogNode.inputContainer.Add (inputPort);
		dialogNode.RefreshExpandedState();
		dialogNode.RefreshPorts();
		dialogNode.SetPosition (new Rect (Vector2.zero, defaultSize));

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
		
		var textField = new TextField
		{
			name = string.Empty,
			value = choicePortName
		};

		textField.RegisterValueChangedCallback (evt => generatedPort.portName = evt.newValue);
		generatedPort.contentContainer.Add (new Label ("  "));
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
}

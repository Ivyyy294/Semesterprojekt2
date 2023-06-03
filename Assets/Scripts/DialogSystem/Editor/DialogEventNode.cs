using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Ivyyy.GameEvent;

public class DialogEventNode : DialogNode
{
    public override void Init(DialogGraphView _parentView)
	{
		base.Init (parentView);

		//Title
		title = data.DialogTitle;
		var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		//titleField.SetValueWithoutNotify (title);
		mainContainer.Add (titleField);

		AddInputPort();

		//Events
		var actionField = new ObjectField("Event");
		actionField.objectType = typeof(GameEvent);
		actionField.RegisterValueChangedCallback(evt => data.GameEvent = evt.newValue as GameEvent);
		actionField.SetValueWithoutNotify (data.GameEvent);
		mainContainer.Add(actionField);

		MarkDirtyRepaint();
		RefreshExpandedState();
		RefreshPorts();
		SetPosition (new Rect (Vector2.zero, defaultSize));
	}

	public static DialogEventNode CreateEventNode(DialogNodeData data, DialogGraphView _parentView = null)
	{
		DialogEventNode node = DialogGraphUtility.CreateEventNode (data);
		node.Init (_parentView);
		return node;
	}

	//Create Raise Event Node
	public static DialogEventNode CreateRaiseEventNode(string nodeName, DialogGraphView _parentView = null)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.RAISE_EVENT
		};

		return CreateEventNode (data, _parentView);
	}

	public static DialogEventNode CreateRaiseEventNode(string nodeName, Vector2 localMousePosition, DialogGraphView _parentView = null)
	{
		DialogEventNode node = CreateRaiseEventNode (nodeName, _parentView);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}

	//Create Listen Event Node
	public static DialogEventNode CreateListenEventNode(string nodeName, DialogGraphView _parentView = null)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.LISTEN_EVENT
		};

		return CreateEventNode (data, _parentView);
	}

	public static DialogEventNode CreateListenEventNode(string nodeName, Vector2 localMousePosition, DialogGraphView _parentView = null)
	{
		DialogEventNode node = CreateListenEventNode (nodeName, _parentView);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}
}

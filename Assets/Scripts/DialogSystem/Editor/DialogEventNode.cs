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

	public static DialogEventNode CreateEventNode(string nodeName, DialogGraphView _parentView = null)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.RAISE_EVENT
		};

		return CreateEventNode (data);
	}

	public static DialogEventNode CreateEventNode(string nodeName, Vector2 localMousePosition, DialogGraphView _parentView = null)
	{
		DialogEventNode node = CreateEventNode (nodeName);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}

	public static DialogEventNode CreateEventNode(DialogNodeData data, DialogGraphView _parentView = null)
	{
		DialogEventNode node = DialogGraphUtility.CreateEventNode (data);
		node.Init (_parentView);
		return node;
	}
}

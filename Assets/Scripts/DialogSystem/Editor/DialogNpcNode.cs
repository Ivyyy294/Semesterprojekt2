using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogNpcNode : DialogNode
{
	public override void Init(DialogGraphView _parentView)
	{
		base.Init (_parentView);

		parentView = _parentView;

		//Title
		title = data.DialogTitle;
		var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		//titleField.SetValueWithoutNotify (title);		
		mainContainer.Add (titleField);

		AddInputPort();

		//Content
		mainContainer.Add (DialogGraphUtility.CreateTextArea (data.DialogText, evt=>{data.DialogText = evt.newValue; MarkDirtyRepaint();}));

		//Image
		Foldout textFoldout = DialogGraphUtility.CreateFoldout ("Image", data.Image == null);
		var actionField = new ObjectField();
		actionField.objectType = typeof(Sprite);
		actionField.RegisterValueChangedCallback(evt => data.Image = evt.newValue as Sprite);
		actionField.SetValueWithoutNotify (data.Image);
		textFoldout.Add (actionField);
		mainContainer.Add(textFoldout);

		MarkDirtyRepaint();
		RefreshExpandedState();
		RefreshPorts();
		SetPosition (new Rect (Vector2.zero, defaultSize));
	}

	public static DialogNpcNode CreateNode(string nodeName, Vector2 localMousePosition, DialogGraphView _parentView = null)
	{
		DialogNpcNode node = CreateNode (nodeName, _parentView);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}

	public static DialogNpcNode CreateNode(string nodeName, DialogGraphView _parentView = null)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			DialogText = "hello world",
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.NPC
		};

		return CreateTextNode (data, _parentView);
	}
	
	public static DialogNpcNode CreateTextNode (DialogNodeData data, DialogGraphView _parentView = null)
	{
		DialogNpcNode node = CreateTextNode (data);

		if (_parentView != null)
			node.Init (_parentView);

		return node;
	}

	public static DialogNpcNode CreateTextNode(DialogNodeData data)
	{
		var dialogNode = new DialogNpcNode {data = data};
		return dialogNode;
	}
}


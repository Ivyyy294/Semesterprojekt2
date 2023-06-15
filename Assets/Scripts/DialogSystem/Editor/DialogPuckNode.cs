using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogPuckNode : DialogNode
{
    public override void Init()
	{
		base.Init ();

		//Title
		title = data.DialogTitle;
		//var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		//mainContainer.Add (titleField);

		AddInputPort();

		//AudioAsset
		var actionField = new ObjectField("AudioAsset");
		actionField.objectType = typeof(AudioAsset);
		actionField.RegisterValueChangedCallback(evt => data.audioAsset = evt.newValue as AudioAsset);
		actionField.SetValueWithoutNotify (data.GameEvent);
		mainContainer.Add(actionField);

		//Emotion
		int indexEmotion = (int) data.emotion;
		mainContainer.Add (DialogGraphUtility.CreateDropDownField ("Emotion", Puck.GetEmotionList(), indexEmotion, onValueChanged => {Enum.TryParse <Puck.Emotion> (onValueChanged.newValue, out data.emotion);}));

		MarkDirtyRepaint();
		RefreshExpandedState();
		RefreshPorts();
		SetPosition (new Rect (Vector2.zero, defaultSize));
	}

	public static DialogPuckNode Create(DialogNodeData data)
	{
		DialogPuckNode node = new DialogPuckNode() {data = data};
		node.Init ();
		return node;
	}

	public static DialogPuckNode Create(string nodeName)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.PUCK,
			emotion = Puck.Emotion.HAPPY
		};

		return Create (data);
	}

	public static DialogPuckNode Create(string nodeName, Vector2 localMousePosition)
	{
		DialogPuckNode node = Create (nodeName);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}
}

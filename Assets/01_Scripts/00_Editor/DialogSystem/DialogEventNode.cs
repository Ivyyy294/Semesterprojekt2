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
}

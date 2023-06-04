using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogChoiceNode : DialogNode
{
	public override void Init(DialogGraphView _parentView)
	{
		parentView = _parentView;

		//Title
		title = data.DialogTitle;
		var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		mainContainer.Add (titleField);

		AddInputPort();

		//Button
		titleContainer.Add (DialogGraphUtility.CreateButton ("New Choice", onClick:() => {CreateChoicePort (); }));

		MarkDirtyRepaint();
		RefreshExpandedState();
		RefreshPorts();
		SetPosition (new Rect (Vector2.zero, defaultSize));
	}

	public void CreateChoicePort(string overriddenPortName = "")
	{
		var generatedPort = DialogGraphUtility.CreatePort (this, Direction.Output);

		//Remove default name label
		var oldLabel = generatedPort.contentContainer.Q<Label>("type");
		generatedPort.contentContainer.Remove (oldLabel);

		var outputPortCount = outputContainer.Query ("connector").ToList().Count;

		string choicePortName = string.IsNullOrEmpty (overriddenPortName)
			? $"Choice {outputPortCount}"
			: overriddenPortName;
		
		//Invisible Label to enable port drag and drop
		generatedPort.contentContainer.Add (new Label ("\t\t"));
		var textField = DialogGraphUtility.CreateTextArea (choicePortName, evt => {generatedPort.portName = evt.newValue; MarkDirtyRepaint();});
		generatedPort.contentContainer.Add (textField);
		
		//Button
		generatedPort.contentContainer.Add (DialogGraphUtility.CreateButton ("X", onClick:() =>parentView.RemovePort (this, generatedPort)));

		generatedPort.portName = choicePortName;

		outputContainer.Add (generatedPort);
		RefreshPorts();
		RefreshExpandedState();		
	}
}

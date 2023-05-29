using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogNode : Node
{
	public string GUID;
	public string dialogText;
	public bool entryPoint = false;
	public static readonly Vector2 defaultSize = new Vector2 (150, 200);
	private DialogGraphView parentView;

	public void Init (DialogGraphView _parentView)
	{
		parentView = _parentView;

		//Title
		var titleField = DialogGraphUtility.CreateTextField ("Title", title, evt=> {title = evt.newValue; MarkDirtyRepaint();});
		//titleField.SetValueWithoutNotify (title);		
		mainContainer.Add (titleField);

		AddInputPort();

		//Button
		titleContainer.Add (DialogGraphUtility.CreateButton ("New Choice", onClick:() => {CreateChoicePort (); }));

		//Content
		Foldout textFoldout = DialogGraphUtility.CreateFoldout ("Dialog Text");
		textFoldout.Add (DialogGraphUtility.CreateTextArea(dialogText, evt=>{dialogText = evt.newValue; MarkDirtyRepaint();}));

		mainContainer.Add (textFoldout);

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
		var textField = DialogGraphUtility.CreateTextField (choicePortName, evt => generatedPort.portName = evt.newValue);
		generatedPort.contentContainer.Add (textField);

		//Button
		generatedPort.contentContainer.Add (DialogGraphUtility.CreateButton ("X", onClick:() =>parentView.RemovePort (this, generatedPort)));

		generatedPort.portName = choicePortName;

		outputContainer.Add (generatedPort);
		RefreshPorts();
		RefreshExpandedState();		
	}

	private void AddInputPort ()
	{
		//Create Input Port
		var inputPort = DialogGraphUtility.CreatePort (this, Direction.Input, Port.Capacity.Multi);
		inputPort.portName = "Input";
		inputContainer.Add (inputPort);
	}
}

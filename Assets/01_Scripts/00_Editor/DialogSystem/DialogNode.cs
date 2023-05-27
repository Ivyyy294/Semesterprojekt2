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

	public void Init ()
	{
		//Title
		var titleField = DialogGraphUtility.CreateTextField ("Title", title, evt=> {title = evt.newValue;});
		titleField.SetValueWithoutNotify (title);		
		mainContainer.Add (titleField);

		//Create Input Port
		var inputPort = DialogGraphUtility.CreatePort (this, Direction.Input, Port.Capacity.Multi);
		inputPort.portName = "Input";

		//Content
		Foldout textFoldout = DialogGraphUtility.CreateFoldout ("Dialog Text");
		textFoldout.Add (DialogGraphUtility.CreateTextArea(dialogText, evt=>{dialogText = evt.newValue;}));

		mainContainer.Add (textFoldout);

		inputContainer.Add (inputPort);
		RefreshExpandedState();
		RefreshPorts();
		SetPosition (new Rect (Vector2.zero, defaultSize));
	}
}

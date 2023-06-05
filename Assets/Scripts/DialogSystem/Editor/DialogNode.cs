using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogNode : Node
{
	//Public Values
	public static readonly Vector2 defaultSize = new Vector2 (150, 200);

	public bool entryPoint = false;
	public DialogNodeData data;
	
	//Public Functions
	public virtual void Init ()
	{
		var generatedPort = DialogGraphUtility.CreatePort (this, Direction.Output);
		generatedPort.portName = "Next";
		outputContainer.Add (generatedPort);
	}

	protected void AddInputPort ()
	{
		//Create Input Port
		var inputPort = DialogGraphUtility.CreatePort (this, Direction.Input, Port.Capacity.Multi);
		inputPort.portName = "Input";
		inputContainer.Add (inputPort);
	}
}

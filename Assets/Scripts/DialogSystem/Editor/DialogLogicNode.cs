using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class DialogLogicNode : DialogNode
{
     public override void Init()
	{
		AddTitleEditField();
		AddInputPort();
		AddOutputPort ("True");
		AddOutputPort ("False");

		Foldout textFoldout = DialogGraphUtility.CreateFoldout ("Settings");

		//Name
		TextField txtName = DialogGraphUtility.CreateTextField ("Name", data.BlackBoardProperty.name, evt=>{data.BlackBoardProperty.name = evt.newValue; MarkDirtyRepaint();});
		txtName.MarkDirtyRepaint();
		textFoldout.Add (txtName);

		//Comparison
		List <string> optionList = new List<string>();
		optionList.Add (BlackBoardProperty.ComparisonTyp.EQUAL.ToString());
		optionList.Add (BlackBoardProperty.ComparisonTyp.GREATER.ToString());
		optionList.Add (BlackBoardProperty.ComparisonTyp.LESS.ToString());

		textFoldout.Add (DialogGraphUtility.CreateDropDownField ("Comparison", optionList, onValueChanged => {Enum.TryParse <BlackBoardProperty.ComparisonTyp> (onValueChanged.newValue, out data.BlackBoardProperty.comparisonTyp);}));

		//Value
		//ToDo check if value is valid
		TextField txtValue = DialogGraphUtility.CreateTextField ("Value", data.BlackBoardProperty.name, evt=>{data.BlackBoardProperty.iVal = int.Parse (evt.newValue); MarkDirtyRepaint();});
		textFoldout.Add (txtValue);

		mainContainer.Add (textFoldout);

		MarkDirtyRepaint();
		RefreshExpandedState();
		RefreshPorts();
	}

	public static DialogLogicNode Create(string nodeName, Vector2 localMousePosition)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.LOGIC
		};

		DialogLogicNode node = Create (data);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}

	public static DialogLogicNode Create (DialogNodeData data)
	{
		DialogLogicNode node = new DialogLogicNode {data = data};
		node.Init ();

		return node;
	}
}

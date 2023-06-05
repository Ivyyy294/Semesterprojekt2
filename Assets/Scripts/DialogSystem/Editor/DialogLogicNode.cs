using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class DialogLogicNode : DialogNode
{
	DialogGraphView dialogGraphView;

    public override void Init ()
	{
		AddTitleEditField();
		AddInputPort();
		AddOutputPort ("True");
		AddOutputPort ("False");

		Foldout textFoldout = DialogGraphUtility.CreateFoldout ("Settings");

		//Name
		int indexName = dialogGraphView.blackBoardProperties.IndexOf (data.BlackBoardProperty.name);
		textFoldout.Add (DialogGraphUtility.CreateDropDownField ("Name", dialogGraphView.blackBoardProperties, indexName, onValueChanged=>{data.BlackBoardProperty.name = onValueChanged.newValue; MarkDirtyRepaint();}));

		//Comparison
		int indexTyp = (int) data.BlackBoardProperty.comparisonTyp;
		textFoldout.Add (DialogGraphUtility.CreateDropDownField ("Comparison", GetComparisonList(), indexTyp, onValueChanged => {Enum.TryParse <BlackBoardProperty.ComparisonTyp> (onValueChanged.newValue, out data.BlackBoardProperty.comparisonTyp);}));

		//Value
		//ToDo check if value is valid
		TextField txtValue = DialogGraphUtility.CreateTextField ("Value", data.BlackBoardProperty.iVal.ToString(), evt=>{data.BlackBoardProperty.iVal = int.Parse (evt.newValue); MarkDirtyRepaint();});
		textFoldout.Add (txtValue);

		mainContainer.Add (textFoldout);

		MarkDirtyRepaint();
		RefreshExpandedState();
		RefreshPorts();
	}

	public static DialogLogicNode Create(string nodeName, Vector2 localMousePosition, DialogGraphView dialogGraphView)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.LOGIC
		};

		DialogLogicNode node = Create (data, dialogGraphView);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}

	public static DialogLogicNode Create (DialogNodeData data, DialogGraphView dialogGraphView)
	{
		DialogLogicNode node = new DialogLogicNode {data = data, dialogGraphView = dialogGraphView};
		node.Init ();

		return node;
	}

	public List <string> GetComparisonList()
	{
		List <string> optionList = new List<string>();
		optionList.Add (BlackBoardProperty.ComparisonTyp.EQUAL.ToString());
		optionList.Add (BlackBoardProperty.ComparisonTyp.GREATER.ToString());
		optionList.Add (BlackBoardProperty.ComparisonTyp.LESS.ToString());
		return optionList;
	}
}

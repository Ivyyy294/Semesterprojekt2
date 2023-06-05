using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogWaitNode : DialogNode
{
	DialogGraphView dialogGraphView;

    public override void Init ()
	{
		AddTitleEditField();
		AddInputPort();
		AddOutputPort ("Next");

		Foldout textFoldout = DialogGraphUtility.CreateFoldout ("Settings");

		//Name
		int indexName = dialogGraphView.blackBoardProperties.IndexOf (data.BlackBoardProperty.name);
		textFoldout.Add (DialogGraphUtility.CreateDropDownField ("Name", dialogGraphView.blackBoardProperties, indexName, onValueChanged=>{data.BlackBoardProperty.name = onValueChanged.newValue; MarkDirtyRepaint();}));

		//Comparison
		int indexTyp = (int) data.BlackBoardProperty.comparisonTyp;
		textFoldout.Add (DialogGraphUtility.CreateDropDownField ("Comparison", BlackBoardProperty.GetComparisonTypList(), indexTyp, onValueChanged => {Enum.TryParse <BlackBoardProperty.ComparisonTyp> (onValueChanged.newValue, out data.BlackBoardProperty.comparisonTyp);}));

		//Value
		//ToDo check if value is valid
		TextField txtValue = DialogGraphUtility.CreateTextField ("Value", data.BlackBoardProperty.iVal.ToString(), evt=>{data.BlackBoardProperty.iVal = int.Parse (evt.newValue); MarkDirtyRepaint();});
		textFoldout.Add (txtValue);

		mainContainer.Add (textFoldout);

		MarkDirtyRepaint();
		RefreshExpandedState();
		RefreshPorts();
	}

	public static DialogWaitNode Create(string nodeName, Vector2 localMousePosition, DialogGraphView dialogGraphView)
	{
		DialogNodeData data = new DialogNodeData
		{
			DialogTitle = nodeName,
			Guid =  System.Guid.NewGuid().ToString(),
			Type = DialogNodeData.NodeType.WAIT
		};

		DialogWaitNode node = Create (data, dialogGraphView);
		node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		return node;
	}

	public static DialogWaitNode Create (DialogNodeData data, DialogGraphView dialogGraphView)
	{
		DialogWaitNode node = new DialogWaitNode {data = data, dialogGraphView = dialogGraphView};
		node.Init ();

		return node;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraph : EditorWindow
{
	private DialogGraphView dialogGraphView;

	[MenuItem ("Graph/Dialog Graph")]
	public static void OpenDialogGraphWindow()
	{
		var window = GetWindow <DialogGraph>();
		window.titleContent = new GUIContent ("Dialog Graph");
	}

	private void OnEnable()
	{
		ConstructGraphView();
		GenerateToolbar();
	}

	private void OnDisable()
	{
		rootVisualElement.Remove (dialogGraphView);
	}

	private void ConstructGraphView()
	{
		dialogGraphView = new DialogGraphView {name = "Dialog Graph" };
		dialogGraphView.StretchToParentSize();
		rootVisualElement.Add (dialogGraphView);
	}

	private void GenerateToolbar()
	{
		var toolbar = new Toolbar();
		var nodeCreateButton = new Button ( clickEvent:() =>
		{
			dialogGraphView.CreateNode("Dialog Node");
		});

		nodeCreateButton.text = "Create Node";
		toolbar.Add (nodeCreateButton);

		rootVisualElement.Add (toolbar);
	}
}
   
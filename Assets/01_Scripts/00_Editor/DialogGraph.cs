using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraph : EditorWindow
{
	private DialogGraphView dialogGraphView;
	private string fileName = "New Narrative"; 

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
		GenerateMiniMap();
	}

	private void GenerateMiniMap()
	{
		var miniMap = new MiniMap {anchored = true };
		miniMap.SetPosition (new Rect (10, 30, 200, 140));
		dialogGraphView.Add (miniMap);
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

		var fileNameTextField = new TextField ("File Name");
		fileNameTextField.SetValueWithoutNotify (fileName);
		fileNameTextField.MarkDirtyRepaint();
		fileNameTextField.RegisterValueChangedCallback (evt => fileName = evt.newValue);
		toolbar.Add(fileNameTextField);

		toolbar.Add (new Button (clickEvent:() => RequestDataOperation(true)) {text = "Save Data"});
		toolbar.Add (new Button (clickEvent:() => RequestDataOperation(false)) {text = "Load Data"});

		var nodeCreateButton = new Button ( clickEvent:() =>
		{
			dialogGraphView.CreateNode("Dialog Node");
		});

		nodeCreateButton.text = "Create Node";
		toolbar.Add (nodeCreateButton);

		rootVisualElement.Add (toolbar);
	}

	private void RequestDataOperation(bool save)
	{
		if (string.IsNullOrEmpty (fileName))
		{
			EditorUtility.DisplayDialog ("Invalid file name!", "Please enter a valid file name.", "OK");
			return;
		}

		var saveUtility = GraphSaveUtility.GetInstance (dialogGraphView);

		if (save)
			saveUtility.SaveGraph (fileName);
		else
			saveUtility.LoadGraph (fileName);
	}
}
   
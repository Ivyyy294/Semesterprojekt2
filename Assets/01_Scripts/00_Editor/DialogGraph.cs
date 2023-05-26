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

	public static void OpenDialogGraphWindow(string assetName)
	{
		if (!string.IsNullOrEmpty (assetName))
		{
			var window = GetWindow <DialogGraph>();
			window.titleContent = new GUIContent ("Dialog Graph [" + assetName + "]");
			window.fileName = assetName;
			window.RequestDataOperation (false);
		}
		else
			EditorUtility.DisplayDialog ("Invalid Asset name!", "Please enter a valid file name.", "OK");
	}

	private void OnEnable()
	{
		ConstructGraphView();
		GenerateToolbar();
		GenerateMiniMap();
	}

	private void OnDisable()
	{
		rootVisualElement.Remove (dialogGraphView);
	}

	private void GenerateMiniMap()
	{
		var miniMap = new MiniMap {anchored = true };
		miniMap.SetPosition (new Rect (10, 30, 200, 140));
		dialogGraphView.Add (miniMap);
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

		//var fileNameTextField = new TextField ("File Name");
		//fileNameTextField.SetValueWithoutNotify (fileName);
		//fileNameTextField.MarkDirtyRepaint();
		//fileNameTextField.RegisterValueChangedCallback (evt => fileName = evt.newValue);
		//toolbar.Add(fileNameTextField);

		toolbar.Add (new Button (clickEvent:() => RequestDataOperation(true)) {text = "Save Data"});
		//toolbar.Add (new Button (clickEvent:() => RequestDataOperation(false)) {text = "Load Data"});

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
   
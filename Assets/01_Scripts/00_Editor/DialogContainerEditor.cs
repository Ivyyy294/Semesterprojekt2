using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogContainer))]
public class DialogContainerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Open Custom Graph Window"))
			DialogGraph.OpenDialogGraphWindow(target.name);
	}
}

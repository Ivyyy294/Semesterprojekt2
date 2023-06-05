using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditBlackBoardValue))]
public class EditBlackBoardValueEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditBlackBoardValue obj = (EditBlackBoardValue) target;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("blackBoardList"));
		EditorGUILayout.Space();

		if (obj.blackBoardList != null)
		{
			SerializedProperty name = serializedObject.FindProperty("propertyName");
			int index = obj.blackBoardList.propertyList.IndexOf (name.stringValue);
			index = EditorGUILayout.Popup ("Name", index, obj.blackBoardList.propertyList.ToArray());
			name.stringValue = obj.blackBoardList.propertyList[index];
			EditorGUILayout.PropertyField(serializedObject.FindProperty("editTyp"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
		}

		serializedObject.ApplyModifiedProperties();
	}
}
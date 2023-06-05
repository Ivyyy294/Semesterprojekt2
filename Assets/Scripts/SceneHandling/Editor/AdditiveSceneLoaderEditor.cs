using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (AdditiveSceneLoader))]
public class AdditiveSceneLoaderEditor : Editor
{
    private SerializedProperty sceneAssetsProperty;
    private SerializedProperty stringListProperty;

    private void OnEnable()
    {
        sceneAssetsProperty = serializedObject.FindProperty("sceneAssets");
        stringListProperty = serializedObject.FindProperty("scenesToLoad");
    }

	public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(sceneAssetsProperty, true);

        if (EditorGUI.EndChangeCheck())
        {
			AddSceneNames();
            serializedObject.ApplyModifiedProperties();
        }
    }

	private void AddSceneNames()
	{
		stringListProperty.ClearArray();

		for (int i = 0; i < sceneAssetsProperty.arraySize; i++)
		{
			SceneAsset sceneAsset = sceneAssetsProperty.GetArrayElementAtIndex(i).objectReferenceValue as SceneAsset;

			if (sceneAsset != null)
			{
				string sceneName = sceneAsset.name;
				stringListProperty.InsertArrayElementAtIndex(i);
				stringListProperty.GetArrayElementAtIndex(i).stringValue = sceneName;
			}
		}
	}
}

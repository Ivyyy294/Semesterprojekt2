using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MusicAsset))]
public class MusicAssetEditor : Editor
{
    public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		MusicAsset audioAsset = (MusicAsset)target;
		
		////Volume
		//EditorGUILayout.Space();
		//string labelVolume = ("Volume \t[" + audioAsset.volume.x.ToString("0.00") + " - " + audioAsset.volume.y.ToString("0.00") + "]");
		//EditorGUILayout.MinMaxSlider (labelVolume, ref audioAsset.volume.x, ref audioAsset.volume.y, 0f, 1f);
		
		////Pitch
		//EditorGUILayout.Space();
		//string labelPitch = ("Pitch \t [" + audioAsset.pitch.x.ToString("0.00") + " - " + audioAsset.pitch.y.ToString("0.00") + "]");
		//EditorGUILayout.MinMaxSlider (labelPitch, ref audioAsset.pitch.x, ref audioAsset.pitch.y, 0f, 2f);

		EditorGUILayout.Space();

		if (GUILayout.Button("Play Preview"))
			audioAsset.Play();

		EditorGUILayout.Space();

		if (GUILayout.Button("Stop Preview"))
			audioAsset.Stop();
	}
}

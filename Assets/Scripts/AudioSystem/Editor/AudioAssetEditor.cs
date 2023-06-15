using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioAsset))]
public class AudioAssetEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		//Volume
		EditorGUILayout.Space();
		AudioAsset audioAsset = (AudioAsset)target;
		string labelVolume = ("Volume \t[" + audioAsset.volume.x.ToString("0.00") + " - " + audioAsset.volume.y.ToString("0.00") + "]");
		EditorGUILayout.MinMaxSlider (labelVolume, ref audioAsset.volume.x, ref audioAsset.volume.y, 0f, 1f);
		
		//Pitch
		EditorGUILayout.Space();
		string labelPitch = ("Pitch \t [" + audioAsset.pitch.x.ToString("0.00") + " - " + audioAsset.pitch.y.ToString("0.00") + "]");
		EditorGUILayout.MinMaxSlider (labelPitch, ref audioAsset.pitch.x, ref audioAsset.pitch.y, 0f, 2f);

		EditorGUILayout.Space();

		if (GUILayout.Button("Play Preview"))
			audioAsset.PlayPreview();

		EditorGUILayout.Space();

		if (GUILayout.Button("Stop Preview"))
			audioAsset.StopPreview();
	}

	//private Texture2D CreateTextureWithColor(Color color)
 //   {
 //       Texture2D texture = new Texture2D(1, 1);
 //       texture.SetPixel(0, 0, color);
 //       texture.Apply();
 //       return texture;
 //   }
}

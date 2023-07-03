using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;
using System.IO;
using System;

public class DebugLog : MonoBehaviour
{
	[SerializeField] Chat [] chats;
	private string filePath;

	private void Start()
	{
		filePath = Application.persistentDataPath + "/DebugLog.txt";
	}

	private void OnDisable()
	{
		StreamWriter writer = new StreamWriter (filePath);
		writer.WriteLine (DateTime.Now.ToString());
		writer.WriteLine ("BlackBoard");

		foreach (var i in BlackBoard.Me().GetProperties())
		{
			string name = i.Value.name;
			string guid = i.Key;
			string data = i.Value.iVal.ToString();
			writer.WriteLine (name + "{ID: " + guid + ", Data: " + data + "}");
		}

		for (int i = 0; i < chats.Length; ++i)
			writer.WriteLine (chats[i].GetPayload("Dialog" + i.ToString()).GetSerializedData());

		writer.Close();
	}
}

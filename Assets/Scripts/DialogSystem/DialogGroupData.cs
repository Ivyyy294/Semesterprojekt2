using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogGroupData
{
	public string Guid;
	public string name;
	public Vector2 Position;
	public List <string> childNodeGuid = new List<string>();
}
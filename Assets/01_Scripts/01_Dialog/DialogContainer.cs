using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogContainer : ScriptableObject
{
	public List <NodeLinkData> nodeLinks = new List<NodeLinkData>();
	public List <DialogNodeData> dialogNodeData = new List<DialogNodeData>();
}

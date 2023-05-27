using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu (menuName = "Dialog")]
public class DialogContainer : ScriptableObject
{
	public List <NodeLinkData> nodeLinks = new List<NodeLinkData>();
	public List <DialogNodeData> dialogNodeData = new List<DialogNodeData>();
	public List <DialogGroupData> dialogGroupData = new List<DialogGroupData>();
}

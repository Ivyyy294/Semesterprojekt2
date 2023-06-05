using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu (menuName = "Dialog")]
public class DialogContainer : ScriptableObject
{
	public List <NodeLinkData> nodeLinks = new List<NodeLinkData>();
	public List <DialogNodeData> dialogNodeData = new List<DialogNodeData>();
	public List <DialogGroupData> dialogGroupData = new List<DialogGroupData>();

	public string GetStartNodeGuid ()
	{
		if (nodeLinks.Count > 0)
			return nodeLinks[0].targetNodeGuid;
		else
			return default (string);
	}

	public DialogNodeData GetDialogNodeData (string guid)
	{
		 DialogNodeData nodeData = null;

		if (!string.IsNullOrEmpty(guid))
			nodeData = dialogNodeData.Find (x=> x.Guid == guid);

		return nodeData;
	}

	public List <NodeLinkData> GetDialogPorts (string guid)
	{
		if (!string.IsNullOrEmpty (guid))
			return nodeLinks.Where(x=> x.baseNodeGuid == guid).ToList();

		return null;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class SavePuckTerminal : SaveableObject
{
	[SerializeField] PuckTerminal puckTerminal;

   	override public Payload GetPayload ()
	{
		Payload payload = new Payload(UniqueId);

		SaveChats (payload);

		return payload;
	}

	override public void LoadObject (Payload val)
	{
		LoadChats (val);
	}

	void SaveChats(Payload payload)
	{
		for (int i = 0; i < puckTerminal.chatObjContainers.Length; ++i)
		{
			Chat chat = puckTerminal.chatObjContainers[i].chatObj;
			string nodeName = "Node" + i.ToString();

			if (chat != null)
			{
				DialogTree.Node[] nodeArray = chat.DialogTree.nodesVisited.ToArray();
				int nodeCount = nodeArray.Length;
				payload.Add ( nodeName + "Count", nodeCount);
				payload.Add (nodeName + "Available", puckTerminal.chatObjContainers[i].available);

				//Save items in reverse order
				for (int j = 0; j < nodeCount; ++j)
				{
					int index = nodeCount -1 - j;
					payload.Add (nodeName + "Chat" + j, chat.DialogTree.nodesVisited.ToArray()[index].data.Guid);
				}
			}
		}
	}

	void LoadChats (Payload val)
	{
		for (int i = 0; i < puckTerminal.chatObjContainers.Length; ++i)
		{
			Chat chat = puckTerminal.chatObjContainers[i].chatObj;
			string nodeName = "Node" + i.ToString();

			int nodeCount = int.Parse (val.data[nodeName + "Count"]);
			puckTerminal.chatObjContainers[i].available = bool.Parse (val.data[nodeName + "Available"]);

			chat.DialogTree.nodesVisited.Clear();

			List <string> nodeList = new List<string>();

			for (int j = 0; j < nodeCount; ++j)
				nodeList.Add (val.data[nodeName + "Chat" + j.ToString()]);

			chat?.LoadSaveGame (nodeList);
		}

	}
}

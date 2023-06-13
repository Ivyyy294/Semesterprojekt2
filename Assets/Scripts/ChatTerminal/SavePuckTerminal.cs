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
		for (int i = 0; i < puckTerminal.dialogList.Length; ++i)
		{
			Chat chat = puckTerminal.GetChatObj (i);
			string nodeName = "Node" + i.ToString();

			if (chat != null)
			{
				//Removes the empty end Node
				if (chat.DialogTree.nodesVisited.Count > 0 && chat.DialogTree.nodesVisited.Peek().data == null)
					chat.DialogTree.nodesVisited.Pop();

				DialogTree.Node[] nodeArray = chat.DialogTree.nodesVisited.ToArray();
				int nodeCount = nodeArray.Length;
				payload.Add ( nodeName + "Count", nodeCount);
				payload.Add (nodeName + "Available", puckTerminal.dialogList[i].available);

				//Save items in reverse order
				for (int j = 0; j < nodeCount; ++j)
				{
					int index = nodeCount -1 - j;
					DialogTree.Node node = chat.DialogTree.nodesVisited.ToArray()[index];
					payload.Add (nodeName + "Chat" + j, node.data.Guid);
				}
			}
		}
	}

	void LoadChats (Payload val)
	{
		for (int i = 0; i < puckTerminal.dialogList.Length; ++i)
		{
			Chat chat = puckTerminal.GetChatObj(i);
			string nodeName = "Node" + i.ToString();

			int nodeCount = int.Parse (val.data[nodeName + "Count"]);
			puckTerminal.dialogList[i].available = bool.Parse (val.data[nodeName + "Available"]);

			chat.DialogTree.nodesVisited.Clear();

			List <string> nodeList = new List<string>();

			for (int j = 0; j < nodeCount; ++j)
				nodeList.Add (val.data[nodeName + "Chat" + j.ToString()]);

			chat?.LoadSaveGame (nodeList);
		}
	}
}

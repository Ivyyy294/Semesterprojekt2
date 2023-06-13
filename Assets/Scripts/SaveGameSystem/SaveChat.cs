using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class SaveChat : SaveableObject
{
	[SerializeField] Chat chat;

	//Public Functions
	override public Payload GetPayload ()
	{
		Payload payload = new Payload(UniqueId);

		if (chat != null)
		{
			DialogTree.Node[] nodeArray = chat.DialogTree.nodesVisited.ToArray();
			int nodeCount = nodeArray.Length;
			payload.Add ("NodeCount", nodeCount);

			//Save items in reverse order
			for (int i = 0; i < nodeCount; ++i)
			{
				int index = nodeCount -1 - i;
				payload.Add ("Node" + i, chat.DialogTree.nodesVisited.ToArray()[index].data.Guid);
			}
		}

		return payload;
	}

	override public void LoadObject (Payload val)
	{
		int nodeCount = int.Parse (val.data["NodeCount"]);

		chat.DialogTree.nodesVisited.Clear();

		List <string> nodeList = new List<string>();

		for (int i = 0; i < nodeCount; ++i)
			nodeList.Add (val.data["Node" + i.ToString()]);

		chat?.LoadSaveGame (nodeList);
	}
}

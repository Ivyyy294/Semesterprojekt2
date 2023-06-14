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
				Payload chatPayload = chat.GetPayload (nodeName);
				chatPayload.Add ("Available", puckTerminal.dialogList[i].available);				
				payload.Add (nodeName, chatPayload.GetSerializedData());
			}

		}
	}

	void LoadChats (Payload val)
	{
		for (int i = 0; i < puckTerminal.dialogList.Length; ++i)
		{
			Chat chat = puckTerminal.GetChatObj(i);
			string nodeName = "Node" + i.ToString();

			string data = val.data[nodeName];
			Payload chatPayload = Payload.GetData(data);
			chat.LoadObject (chatPayload);
			puckTerminal.dialogList[i].available = bool.Parse (chatPayload.data["Available"]);
		}
	}
}

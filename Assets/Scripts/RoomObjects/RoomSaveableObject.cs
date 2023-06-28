using UnityEngine;
using Ivyyy.SaveGameSystem;
using System;

public class RoomSaveableObject : SaveableObject
{
	[SerializeField] Room room;
	override public Payload GetPayload ()
	{
		Payload payload = new Payload (UniqueId);
		payload.Add ("currentDay", room.currentDay.ToString());
		return payload;
	}

	override public void LoadObject (Payload val)
	{
		Enum.TryParse (val.data["currentDay"], out room.currentDay);
	}
}

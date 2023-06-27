using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class DoorSaveableObject : SaveableObject
{
	[SerializeField] Door door;

    override public Payload GetPayload ()
	{
		Payload payload = new Payload(UniqueId);

		if (door != null)
		{
			payload.Add ("open", door.open);
			payload.Add ("glitch", door.glitch);
		}
		else
			Debug.LogError("Missing Door!");

		return payload;
	}

	override public void LoadObject (Payload val)
	{
		if (door != null)
		{
			door.open = val.GetBool ("open");
			door.open = val.GetBool ("glitch");
		}
		else
			Debug.LogError("Missing Door!");
	}
}

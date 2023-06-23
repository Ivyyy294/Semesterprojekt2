using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class KitchenSaveableObject : SaveableObject
{
	[SerializeField] Kitchen kitchen;

	override public Payload GetPayload ()
	{
		Payload payload = new Payload(UniqueId);

		if (kitchen != null)
			payload.Add ("active", kitchen.IsActive());
		else
			Debug.LogError("Missing Kitchen!");

		return payload;
	}

	override public void LoadObject (Payload val)
	{
		if (val.GetBool("active"))
			kitchen.Interact();
	}
}

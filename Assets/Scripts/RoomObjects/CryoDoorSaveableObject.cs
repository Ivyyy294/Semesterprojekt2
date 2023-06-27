using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class CryoDoorSaveableObject : SaveableObject
{
	[SerializeField] CryoDoor cryoDoor;
    // Start is called before the first frame update
	override public Payload GetPayload ()
	{
		Payload payload = new Payload (UniqueId);
		payload.Add ("open", cryoDoor.Open);
		return payload;
	}

	override public void LoadObject (Payload val)
	{
		if (val.GetBool("open"))
			cryoDoor.SpawnOpen();
	}
}

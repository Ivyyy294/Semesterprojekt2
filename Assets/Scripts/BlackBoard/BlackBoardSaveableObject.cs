using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class BlackBoardSaveableObject : SaveableObject
{
    override public Payload GetPayload ()
	{
		Payload payload = new Payload (UniqueId);

		foreach (var i in BlackBoard.Me().GetProperties())
			payload.Add (i.Key, i.Value.iVal);

		return payload;
	}

	override public void LoadObject (Payload val)
	{
		foreach (var i in val.data)
			BlackBoard.Me().EditValue (i.Key, BlackBoard.EditTyp.SET, int.Parse(i.Value));
	}
}

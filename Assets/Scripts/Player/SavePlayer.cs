using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;
using Ivyyy.Core;

public class SavePlayer : SaveableObject
{
	[SerializeField] PlayerMovement3D player;
	[SerializeField] MouseLook mouseLook;

	public override Payload GetPayload()
	{
		Payload p = new Payload(UniqueId);
		//Position
		p.Add ("posX", transform.position.x);
		p.Add ("posY", transform.position.y);
		p.Add ("posZ", transform.position.z);

		//Rotation
		//p.Add ("rotX", mouseLook.GetRotationX());
		p.Add ("rotY", mouseLook.GetRotationY());

		return p;
	}

	public override void LoadObject(Payload val)
	{
		//Position
		Vector3 loadedPos = new Vector3();
		loadedPos.x = float.Parse(val.data["posX"]);
		loadedPos.y = float.Parse(val.data["posY"]);
		loadedPos.z = float.Parse(val.data["posZ"]);
		player.SetPosition(loadedPos);

		//Rotation
		//mouseLook.SetRotationX (float.Parse(val.data["rotX"]));
		mouseLook.SetRotationY (float.Parse(val.data["rotY"]));
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
	[System.Serializable]
	public struct CursorData
	{
		public Texture2D texture;
		public Vector2 hotspot;
	}
	[SerializeField] CursorData normal;
	[SerializeField] CursorData hover;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor (normal.texture, normal.hotspot, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
		if (Cursor.lockState == CursorLockMode.Locked)
			Cursor.visible = false;
		else
		{
			Cursor.visible = true;

			if (CursorHoverHandler.IsHovering)
				Cursor.SetCursor (hover.texture, hover.hotspot, CursorMode.ForceSoftware);
			else	
				Cursor.SetCursor (normal.texture, normal.hotspot, CursorMode.ForceSoftware);
		}
}
}

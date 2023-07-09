using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	static bool isHovering;
	public static bool IsHovering => isHovering;

	bool active = false;

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovering = true;
		active = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovering = false;
		active = false;
	}

	private void OnDisable()
	{
		if (active)
			isHovering = false;
	}
}

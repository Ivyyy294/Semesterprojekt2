using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
	[SerializeField] RectTransform rectTransform;
	[SerializeField] Vector2 offset;
	[SerializeField] float speed;
	[SerializeField] GameEvent menuEvent;
	Vector2 startPos;


	private void OnEnable()
	{
		startPos = rectTransform.anchoredPosition;
	}

	public void ShowMenu()
	{
		rectTransform.anchoredPosition = startPos;
		menuEvent?.Raise();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			ShowMenu();

		Vector2 destPos = startPos + offset;

		if (rectTransform.anchoredPosition != destPos)
			rectTransform.anchoredPosition = Vector2.MoveTowards (rectTransform.anchoredPosition, destPos, speed * Time.deltaTime);
		else
			ShowMenu();
	}
}

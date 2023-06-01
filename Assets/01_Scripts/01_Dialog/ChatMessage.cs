using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMessage : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI txtField;
	[SerializeField] float respondTime;
	
	public bool Done=>done;

	private bool done;
	private float timer;
	private string newText;

    public void SetText (string text)
	{
		newText = text;
		timer = 0f;
	}

	private void Update()
	{
		if (timer < respondTime)
			timer += Time.deltaTime;
		else if (!string.IsNullOrEmpty(newText)
			&& txtField != null)
		{
			txtField.text = newText;
			newText = null;
		}
	}
}

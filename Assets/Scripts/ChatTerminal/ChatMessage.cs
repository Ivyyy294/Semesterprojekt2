using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI txtField;
	[SerializeField] float respondTime;
	[SerializeField] Image imageObj;
	
	public bool Done=>done;

	private bool done;
	private float timer;
	private DialogNodeData nodeData = null;

	public void SetContent (DialogNodeData data, bool force = false)
	{
		done = false;
		nodeData = data;
		timer = force ? respondTime : 0f;
	}

	public void SetContent (string text, bool force = false)
	{
		done = false;
		nodeData = new DialogNodeData() {DialogText = text};
		timer = force ? respondTime : 0f;
	}

	private void Update()
	{
		if (timer < respondTime)
			timer += Time.deltaTime;
		else if (!done)
		{
			if (nodeData != null)
			{
				txtField.text = nodeData.DialogText;
				imageObj.gameObject.SetActive (true);

				if (nodeData.Image != null)
				{
					imageObj.gameObject.SetActive (true);
					imageObj.sprite = nodeData.Image;
				}
			}
			
			Canvas.ForceUpdateCanvases();
			done = true;
		}
	}
}

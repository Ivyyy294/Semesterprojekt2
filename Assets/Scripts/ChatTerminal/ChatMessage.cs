using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
	[SerializeField] float minRespondTime = 0.5f;
	[SerializeField] float timePerChar = 0.1f;
	[SerializeField] AudioAsset audioAsset;

	[Header ("Lara Values")]
	[SerializeField] TextMeshProUGUI txtField;
	[SerializeField] Image imageObj;
	
	public bool Done=>done;
	private bool done;
	private float timer;
	private DialogNodeData nodeData = null;
	private float respondTime = 0f;

	//Public Functions
	public void SetContent (DialogNodeData data, bool force = false)
	{
		done = false;
		nodeData = data;
		respondTime = CalculateRespondTime ();
		timer = force ? respondTime : 0f;
	}

	public void SetContent (string text, bool force = false)
	{
		nodeData = new DialogNodeData() {DialogText = text};
		SetContent (nodeData, force);
	}

	//Private Functions
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
				audioAsset?.Play();

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

	private float CalculateRespondTime()
	{
		if (nodeData.customRespondTime > 0f)
			return nodeData.customRespondTime;
		else
			return minRespondTime + nodeData.DialogText.Length * timePerChar;
	}
}

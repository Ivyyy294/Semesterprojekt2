using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;
using UnityEngine.UI;

[RequireComponent(typeof (GameEventListener))]
public class DisconnectedEvent : MonoBehaviour
{
	[SerializeField] float fadeTime = 5f;
	[SerializeField] GameEvent eventCredits;
	[SerializeField] Image img;
	[SerializeField] AudioAsset audioAsset;
	[SerializeField] GameObject terminalUi;
	float timer = 0f;

	private void OnEnable()
	{
		timer = 0f;
		audioAsset.PlayAtPos(transform.position);
		Player.Me().BlockInteractions (true, "Disconnected");
	}

	// Update is called once per frame
	void Update()
    {
		if (!terminalUi.activeInHierarchy)
		{
			img.gameObject.SetActive(true);

			if (timer <= fadeTime)
			{
				ChangeAlpha (timer / fadeTime);
				timer += Time.deltaTime;
			}
			else
			{
				eventCredits?.Raise();
				gameObject.SetActive(false);
			}
		}
    }

	void ChangeAlpha (float val)
	{
		if (img != null)
		{
			Color color = img.color;
			color.a = val;
			img.color = color;
		}
	}
}

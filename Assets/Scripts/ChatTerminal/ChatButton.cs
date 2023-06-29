using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatButton : MonoBehaviour
{
	public bool available = true;
	[SerializeField] GameObject notificationIcon;
	[SerializeField] TextMeshProUGUI notificationText;
	[SerializeField] Chat chat;
	[HideInInspector] public Button button;

	public Chat GetChat() { return chat;}

	public void ShowNotification (bool val)
	{
		notificationIcon.SetActive (val);
	}

	public void ShowChat (bool val)
	{
		chat.gameObject.SetActive(val);
	}
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent <Button>();

		ShowNotification (false);
    }

    // Update is called once per frame
    void Update()
    {
		if (button.interactable != available)
			button.interactable = available;

		if (available)
			ShowNotification (chat.NpcMessageAvailable() && !chat.gameObject.activeInHierarchy);
    }
}

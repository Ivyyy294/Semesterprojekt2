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
	[SerializeField] TextMeshProUGUI ChatNameObj;
	[SerializeField] string chatName;
	[HideInInspector] public Button button;

	public Chat GetChat() { return chat;}

	public void ShowNotification (int anz)
	{
		notificationIcon.SetActive (anz > 0);
		notificationText.text = anz.ToString();
	}

	public void ShowChat (bool val)
	{
		if (val)
			ChatNameObj.text = chatName;

		chat.gameObject.SetActive(val);
	}
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent <Button>();

		ShowNotification (0);
    }

    // Update is called once per frame
    void Update()
    {
		if (button.interactable != available)
			button.interactable = available;

		if (available && !chat.gameObject.activeInHierarchy)
			ShowNotification (chat.AnzNewMessagesAvailable());
		else
			ShowNotification (0);
    }
}

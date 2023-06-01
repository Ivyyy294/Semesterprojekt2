using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
	[SerializeField] DialogContainer dialogContainer;

	[Header ("Lara values")]
	[SerializeField] GameObject messageContainer;
	[SerializeField] GameObject messageNpcTemplate;
	[SerializeField] GameObject messagePlayerTemplate;

	[SerializeField] GameObject buttonContainer;

	private List <GameObject> buttonList = new List<GameObject>();
	private DialogNodeData currentNode;
	private List <NodeLinkData> portList;

    // Start is called before the first frame update
    void Start()
    {
        if (dialogContainer != null)
		{
			for (int i = 0; i < buttonContainer.transform.childCount; ++i)
				buttonList.Add (buttonContainer.transform.GetChild(i).gameObject);

			LoadDialog (dialogContainer.GetStartNodeGuid());
		}
    }

	public void OnButtonPressed (int index)
	{
		if (index > -1 && index < portList.Count)
			LoadDialog (portList[index].targetNodeGuid);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	void LoadDialog (string guid)
	{
		currentNode = dialogContainer.GetDialogNodeData (guid);
		portList = dialogContainer.GetDialogPorts (currentNode.Guid);

		SpawnMessage (currentNode.DialogText);
		//if (txtDialog != null)
		//	txtDialog.text = currentNode.DialogText;

		foreach (var i in buttonList)
		{
			i.gameObject.SetActive (false);
			i.GetComponent <Button>().onClick.RemoveAllListeners();
		}

		for (int i = 0; i < portList.Count; ++i)
		{
			NodeLinkData port = portList[i];
			buttonList[i].gameObject.SetActive(true);
			buttonList[i].GetComponent<Button>().onClick.AddListener (call:() =>{SpawnPlayerMessage(port);});
			buttonList[i].GetComponentInChildren<TextMeshProUGUI>().text = port.portName;
		}
	}

	void SpawnPlayerMessage (NodeLinkData port)
	{
		SpawnMessage (port.portName, true);
		LoadDialog(port.targetNodeGuid);
	}

	void SpawnMessage (string text, bool player = false)
	{
		ChatMessage msg;

		if (player)
			msg = Instantiate (messagePlayerTemplate, messageContainer.transform).GetComponent <ChatMessage>();
		else
			msg = Instantiate (messageNpcTemplate, messageContainer.transform).GetComponentInChildren<ChatMessage>();

		msg?.SetText (text);
	}
}

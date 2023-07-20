using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtitleData
{
	public string text;
	public float displayTime;
	public int priority;
	public float timer = 0f;
}

public class Subtitle : MonoBehaviour
{
	private static Subtitle me;

	[SerializeField] TextMeshProUGUI textObj;
	private SubtitleData dataQueue;

	public static void Clear()
	{
		me.dataQueue = null;
	}

	public static void Add (string text, float _displayTime = 0.5f, int priority = 0)
	{
		if (me != null)
		{
			if (me.dataQueue == null || priority >= me.dataQueue.priority)
				me.dataQueue = new SubtitleData {text = text, displayTime = _displayTime, priority = priority}; 
		}
	}

	//public static Subtitle Me()
	//{
	//	if (me == null)
	//	{
	//		me = new GameObject ("Subtitle", typeof (Subtitle)).GetComponent<Subtitle>();
	//		me.gameObject.hideFlags = HideFlags.HideAndDontSave;
	//		DontDestroyOnLoad (me);
	//	}

	//	return me;
	//}

    // Start is called before the first frame update
    void Start()
    {
		if (me == null)
			me = this;
		else if (me != null && me != this)
			Destroy (this.gameObject);

		GetComponent<Canvas>().sortingOrder = 2;
    }

    // Update is called once per frame
    void Update()
    {
		if (dataQueue == null || !GameSettings.Me().audioSettings.subtitle)
			textObj.gameObject.transform.parent.gameObject.SetActive (false);
		else
		{
			if (!textObj.gameObject.activeInHierarchy)
				textObj.gameObject.transform.parent.gameObject.SetActive (true);

			SubtitleData current = dataQueue;

			if (current.timer < current.displayTime)
			{
				if (current.timer == 0f)
				{
					if (string.IsNullOrEmpty(current.text))
						textObj.SetText ("");
					else
						textObj.SetText ("[" + current.text + "]");

					Canvas.ForceUpdateCanvases();
				}
				
				current.timer += Time.deltaTime;
			}
			else
				dataQueue = null;
		}
    }
}

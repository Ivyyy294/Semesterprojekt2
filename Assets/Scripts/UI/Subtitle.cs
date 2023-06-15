using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Subtitle : MonoBehaviour
{
	private static List <Subtitle> instances = new List<Subtitle>();

	[SerializeField] TextMeshProUGUI textObj;
	[SerializeField] float lifeTime = 0.5f;
	private float timer;

	public static void SetText (string text)
	{
		foreach (Subtitle i in instances)
		{
			i.textObj.text = text;
			i.textObj.gameObject.SetActive(true);
			i.timer = 0f;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        instances.Add (this);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < lifeTime)
			timer += Time.deltaTime;
		else if (textObj.gameObject.activeInHierarchy)
			textObj.gameObject.SetActive (false);
    }

	private void OnDestroy()
	{
		instances.Remove(this);
	}
}

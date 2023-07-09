using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DateTimeUi : MonoBehaviour
{
	static string date = "2605.01.01";
	static string time = "2:30AM";
	[SerializeField] TextMeshProUGUI dateTxt;
	[SerializeField] TextMeshProUGUI timeTxt;

	public static void SetDate(string val) { date = val;}

    // Start is called before the first frame update
    void Start()
    {
        dateTxt.text = date;
		timeTxt.text = time;
    }

	private void Update()
	{
		dateTxt.text = date;
		timeTxt.text = time;
	}
}

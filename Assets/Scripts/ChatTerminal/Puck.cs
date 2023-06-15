using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Puck : MonoBehaviour
{
	public enum Emotion
	{
		HAPPY,
		SAD,
		PSYCHOTIC,
		CHEEKY,
		ANNOYED,
		WARNING,
		BORED,
		DISGUSTED,
		INTERESTED,
		SHOCKED,
		ANGRY
	}
   
	public static List <string> GetEmotionList()
	{
		List <string> optionList = new List<string>();

		foreach(var i in Enum.GetValues(typeof(Emotion)))
			optionList.Add (i.ToString());

		return optionList;
	}
}

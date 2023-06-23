using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Puck
{
	public enum Emotion
	{
		ANGRY,
		ANNOYED,
		BORED,
		BUSY,
		CHEEKY,
		CONFUSED,
		CRACKING,
		CRAZY,
		CRYING,
		DISGUSTED,
		ERROR,
		FLIRT,
		HAPPY,
		LOVE,
		INTERESTED,
		LAUGHING,
		SHOCKED,
		WARNING
	}
   
	static Emotion currentEmotion = Emotion.HAPPY;
	static List <PuckInterface> interfaces = new List <PuckInterface>();

	//Public Functions
	public static void SetEmotion (Emotion emotion)
	{
		currentEmotion = emotion;

		foreach (PuckInterface i in interfaces)
			i.OnEmotionChanged (currentEmotion);
	}

	public static List <string> GetEmotionList()
	{
		List <string> optionList = new List<string>();

		foreach(var i in Enum.GetValues(typeof(Emotion)))
			optionList.Add (i.ToString());

		return optionList;
	}

	public static void RegisterInterface (PuckInterface puckInterface)
	{
		//Makes sure the Interfaces start with the active emotion
		puckInterface.OnEmotionChanged (currentEmotion);
		interfaces.Add (puckInterface);
	}

	public static void DeregisterInterface (PuckInterface puckInterface)
	{
		if (interfaces.Contains(puckInterface))
			interfaces.Remove (puckInterface);
	}
}

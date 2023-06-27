using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
	private static GameSettings me;
	public AudioSettings audioSettings;

	private GameSettings()
	{
		audioSettings = new AudioSettings();
	}

	//Public Functions
	public static GameSettings Me()
	{
		if (me == null)
			me = new GameSettings();

		return me;
	}
}

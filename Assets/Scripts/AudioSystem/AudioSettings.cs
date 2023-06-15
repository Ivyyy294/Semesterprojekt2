using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings
{
	private static AudioSettings me;

	public float sfxVolume;
	public float musicVolume;
	public float ambientVolume;
	public float uiVolume;

	//Public Functions
	public static AudioSettings Me()
	{
		if (me == null)
			me = new AudioSettings();

		return me;
	}

	public void SaveSettings()
	{
		PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
		PlayerPrefs.SetFloat("musicVolume", musicVolume);
		PlayerPrefs.SetFloat("ambientVolume", ambientVolume);
		PlayerPrefs.SetFloat("uiVolume", uiVolume);
        PlayerPrefs.Save();
	}

    //Private Functions
    AudioSettings()
    {
		LoadSettings();
    }

	void LoadSettings()
	{
		sfxVolume = LoadValue ("sfxVolume");
		musicVolume = LoadValue ("musicVolume");
		ambientVolume = LoadValue ("ambientVolume");
		uiVolume = LoadValue ("uiVolume");
	}


	float LoadValue (string key)
	{
		if (PlayerPrefs.HasKey (key))
			return PlayerPrefs.GetFloat (key);
		else
			return 1f;
	}
}

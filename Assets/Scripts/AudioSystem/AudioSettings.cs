using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings
{
	private static AudioSettings me;

	public float sfxVolume = 1f;
	public float musicVolume = 1f;
	public float ambientVolume = 1f;
	public float uiVolume = 1f;
	public float voiceLine = 1f;

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
		PlayerPrefs.SetFloat("voiceLine", voiceLine);
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
		voiceLine = LoadValue ("voiceLine");
	}


	float LoadValue (string key)
	{
		if (PlayerPrefs.HasKey (key))
			return PlayerPrefs.GetFloat (key);
		else
			return 1f;
	}
}

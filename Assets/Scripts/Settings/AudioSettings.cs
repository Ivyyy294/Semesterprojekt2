using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings
{
	public float sfxVolume = 1f;
	public float musicVolume = 1f;
	public float ambientVolume = 1f;
	public float uiVolume = 1f;
	public float voiceLine = 1f;
	public bool subtitle = true;

	public void SaveSettings()
	{
		PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
		PlayerPrefs.SetFloat("musicVolume", musicVolume);
		PlayerPrefs.SetFloat("ambientVolume", ambientVolume);
		PlayerPrefs.SetFloat("uiVolume", uiVolume);
		PlayerPrefs.SetFloat("voiceLine", voiceLine);
		PlayerPrefs.SetFloat ("subtitle", subtitle ? 1f : 0f);
        PlayerPrefs.Save();
	}

    public AudioSettings()
    {
		LoadSettings();
    }

    //Private Functions
	void LoadSettings()
	{
		sfxVolume = LoadValue ("sfxVolume");
		musicVolume = LoadValue ("musicVolume");
		ambientVolume = LoadValue ("ambientVolume");
		uiVolume = LoadValue ("uiVolume");
		voiceLine = LoadValue ("voiceLine");
		subtitle = LoadValue ("subtitle") > 0f ? true: false;
	}

	float LoadValue (string key)
	{
		if (PlayerPrefs.HasKey (key))
			return PlayerPrefs.GetFloat (key);
		else
			return 1f;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUi : MonoBehaviour
{
	[SerializeField] Slider musicSetting;
	[SerializeField] Slider ambientSetting;
	[SerializeField] Slider sfxSetting;
	[SerializeField] Slider voiceSetting;
	[SerializeField] Slider uiSetting;
	[SerializeField] SwitchToggle subtitle;

	AudioSettings audioSettings;

	private void OnEnable()
	{
		audioSettings = AudioSettings.Me();
		musicSetting.value = audioSettings.musicVolume * 10f;
		ambientSetting.value = audioSettings.ambientVolume * 10f;
		sfxSetting.value = audioSettings.sfxVolume * 10f;
		voiceSetting.value = audioSettings.voiceLine * 10f;
		uiSetting.value = audioSettings.uiVolume * 10f;
		subtitle.active = audioSettings.subtitle;
	}

	private void OnDisable()
	{
		Save();
	}

	public void Save()
	{
		audioSettings.musicVolume = musicSetting.value * 0.1f;
		audioSettings.ambientVolume = ambientSetting.value * 0.1f;
		audioSettings.sfxVolume = sfxSetting.value * 0.1f;
		audioSettings.voiceLine = voiceSetting.value * 0.1f;
		audioSettings.uiVolume = uiSetting.value * 0.1f;
		audioSettings.subtitle = subtitle.active;
		audioSettings.SaveSettings();
	}
}

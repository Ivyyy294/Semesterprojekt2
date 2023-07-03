using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class GraphicsSettingsUi : MonoBehaviour
{
	[SerializeField] SwitchToggle fullscreen;
	[SerializeField] TMP_Dropdown resolutionDropDown;

	Resolution[] resolutions;
	GraphicSettings graphicSettings;
	int currentIndex;

	private void OnEnable()
	{
		currentIndex = -1;

		resolutions = Screen.resolutions;
		graphicSettings = GameSettings.Me().graphicSettings;
		fullscreen.active = graphicSettings.fullscreen;

		resolutionDropDown.options.Clear();

		for (int i = 0; i < resolutions.Length; ++i)
		{
			Resolution tmp = resolutions[i];
			resolutionDropDown.options.Add(new TMP_Dropdown.OptionData(tmp.ToString()));
		}

		int index = Array.IndexOf (resolutions, graphicSettings.resolution);
		
		if (index != -1)
		{
			Debug.Log ("Saved res");
			resolutionDropDown.value = index;
			currentIndex = index;
		}
		else
		{
			Debug.Log ("Default res");
			resolutionDropDown.value = resolutions.Length -1;
		}
	}

	private void Update()
	{
		if (fullscreen.active != graphicSettings.fullscreen)
			graphicSettings.fullscreen = fullscreen.active;

		if (resolutionDropDown.value != currentIndex)
		{
			currentIndex = resolutionDropDown.value;
			Resolution tmp = resolutions[currentIndex];
			Screen.SetResolution (tmp.width, tmp.height, graphicSettings.fullscreen, tmp.refreshRate);
		}
	}

	private void OnDisable()
	{
		Save();
	}

	public void OnFullscreenToggle()
	{
		Screen.fullScreen = fullscreen.active;
	}

	public void Save()
	{
		graphicSettings.fullscreen = fullscreen.active;
		graphicSettings.resolution = resolutions[currentIndex];
		graphicSettings.SaveSettings();
	}
}

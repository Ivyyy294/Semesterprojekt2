using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSettings : ISettingContainer
{
	public bool fullscreen = true;
	public Resolution resolution;

	public GraphicSettings() { LoadSettings();}

	public void SaveSettings()
	{
		Debug.Log ("SaveSettings");
		PlayerPrefs.SetInt ("IvyyyFullscreen", fullscreen ? 1 : 0);
		PlayerPrefs.SetInt ("IvyyyresolutionW", resolution.width);
		PlayerPrefs.SetInt ("IvyyyresolutionH", resolution.height);
		PlayerPrefs.SetInt ("IvyyyresolutionHz", resolution.refreshRate);
	}

	public void LoadSettings()
	{
		Debug.Log ("LoadSettings");
		fullscreen = PlayerPrefs.GetInt("IvyyyFullscreen") > 0 ? true : false;
		resolution.width = PlayerPrefs.GetInt("IvyyyresolutionW");
		resolution.width = PlayerPrefs.GetInt("IvyyyresolutionH");
		resolution.width = PlayerPrefs.GetInt("IvyyyresolutionHz");

		if (resolution.width == 0 || resolution.height == 0)
			resolution = Screen.currentResolution;
	}
}

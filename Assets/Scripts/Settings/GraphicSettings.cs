using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSettings : ISettingContainer
{
	public bool fullscreen = true;

	public GraphicSettings() { LoadSettings();}

	public void SaveSettings()
	{
		PlayerPrefs.SetInt ("IvyyyFullscreen", fullscreen ? 1 : 0);
	}

	public void LoadSettings()
	{
		fullscreen = PlayerPrefs.GetInt("IvyyyFullscreen") > 0 ? true : false;
	}
}

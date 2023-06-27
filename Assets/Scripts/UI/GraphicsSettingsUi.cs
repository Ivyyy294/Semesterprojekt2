using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettingsUi : MonoBehaviour
{
	[SerializeField] SwitchToggle fullscreen;

	GraphicSettings graphicSettings;

	private void OnEnable()
	{
		graphicSettings = GameSettings.Me().graphicSettings;
		fullscreen.active = graphicSettings.fullscreen;
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
		graphicSettings.SaveSettings();
	}
}

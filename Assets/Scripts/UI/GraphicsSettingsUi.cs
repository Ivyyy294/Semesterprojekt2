using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GraphicsSettingsUi : MonoBehaviour
{
	[SerializeField] SwitchToggle fullscreen;
	//[SerializeField] Dropdown resolution;

	GraphicSettings graphicSettings;

	private void OnEnable()
	{
		graphicSettings = GameSettings.Me().graphicSettings;
		fullscreen.active = graphicSettings.fullscreen;
		//resolution.options.Clear();

		//foreach (var i in Screen.resolutions)
		//	resolution.options.Add (new Dropdown.OptionData (i.ToString()));
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

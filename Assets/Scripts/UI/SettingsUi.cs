using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUi : MonoBehaviour
{
	[SerializeField] GameObject audioSettings;
	[SerializeField] GameObject graphicSettings;
	GameObject currentSetting;

	public void ShowAudioSettings()
	{
		ShowSetting (audioSettings);
	}

	public void ShowGraphicSettings()
	{
		ShowSetting (graphicSettings);
	}

    // Start is called before the first frame update
    void Start()
    {
        ShowSetting (audioSettings);
    }

	void ShowSetting (GameObject newSetting)
	{
		if (currentSetting != null)
			currentSetting.SetActive(false);

		currentSetting = newSetting;
		currentSetting.SetActive (true);
	}
}

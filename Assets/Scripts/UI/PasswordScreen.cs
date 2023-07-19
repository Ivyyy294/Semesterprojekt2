using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasswordScreen : MonoBehaviour
{
	[SerializeField] string password;
	[SerializeField] AudioAsset audioAssetClick;
	[SerializeField] AudioAsset audioAssetAcessDenied;
	
	[Header ("Enter Animation")]
	[SerializeField] float enterTime;

	[Header ("Passwort clue")]
	[SerializeField] GameObject puck;
	[SerializeField] int attempts;

	[Header ("Lara values")]
	[SerializeField] GameObject chatScreen;
	[SerializeField] GameObject parent;
	[SerializeField] TMP_InputField inputField;

	int wrongPasswordCounter;
	float timer = 0f;
	bool passwordAvailable = true;

	public void CheckPassword()
	{
		if (inputField.text == password)
		{
			passwordAvailable = true;
			inputField.text = "";
			ShowTerminal();
		}
		else if (!passwordAvailable)
		{
			audioAssetAcessDenied.PlayAtPos (Camera.main.transform.position);
			inputField.text = "";
			++wrongPasswordCounter;
		}
	}

	void ShowTerminal()
	{
		audioAssetClick.Play();
		chatScreen.SetActive(true);
		parent.SetActive(false);
	}

	public void SetPasswordAvailable(bool val)
	{
		passwordAvailable = val;
	}

	private void OnEnable()
	{
		timer = 0f;
		chatScreen.SetActive(false);
		inputField.readOnly = passwordAvailable;
		inputField.text = "";
	}

	private void Update()
	{
		if (passwordAvailable)
		{
			if (timer <= enterTime)
			{
				float timePerChar = enterTime / password.Length;
				int index = (int)(timer / timePerChar);
				string tmpPassword = "";

				for (int i = 0; i < index + 1; ++i)
					tmpPassword += "*";

				inputField.text = tmpPassword;
				timer += Time.deltaTime;
			}
			else
				ShowTerminal();
		}

		puck.SetActive (!passwordAvailable && wrongPasswordCounter >= attempts);
	}
}

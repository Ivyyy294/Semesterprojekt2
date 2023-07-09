using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasswordScreen : MonoBehaviour
{
	[SerializeField] string password;
	[SerializeField] float enterTime;
	[SerializeField] AudioAsset audioAssetClick;
	[SerializeField] AudioAsset audioAssetAcessDenied;
	[SerializeField] GameObject chatScreen;
	[SerializeField] GameObject img;
	[SerializeField] GameObject parent;
	[SerializeField] TextMeshProUGUI textObj;

	float timer;
	bool passwordAvailable = true;
	bool playAnimation = false;
	bool firstTime = true;
	
	public void SetPasswordAvailable(bool val)
	{
		passwordAvailable = val;
	}

	private void OnEnable()
	{
		timer = 0f;

		if (passwordAvailable && !firstTime)
			StartAnimation();
		else
			img.SetActive(false);
	}

	public void Interact()
	{
		if (!passwordAvailable)
			audioAssetAcessDenied.Play();
		else if (firstTime)
			StartAnimation();
	}

	private void StartAnimation()
	{
		textObj.text = "";
		playAnimation = true;
		img.SetActive (true);
	}

	private void Update()
	{
		if (playAnimation)
		{
			if (firstTime)
				firstTime = false;

			if (timer <= enterTime)
			{
				float timePerChar = enterTime / password.Length;
				int index = (int) (timer / timePerChar);
				textObj.text = password.Substring (0, index);
				timer += Time.deltaTime;
			}
			else
			{
				playAnimation = false;
				audioAssetClick.Play();
				chatScreen.SetActive(true);
				parent.SetActive(false);
			}
		}
	}
}

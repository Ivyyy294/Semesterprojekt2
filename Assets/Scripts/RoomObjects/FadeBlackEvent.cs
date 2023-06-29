using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ivyyy.Interfaces;

[RequireComponent (typeof(AudioPlayer))]
public class FadeBlackEvent : MonoBehaviour, InteractableObject
{
	[SerializeField] Image img;
	[SerializeField] AnimationCurve fadeInCurve;
	[SerializeField] AnimationCurve fadeOutCurve;
	AudioPlayer audioPlayer;

	bool fadeIn;
	bool active = false;
	float timer;

	public void Interact()
	{
		active = true;
		fadeIn = true;
		timer = 0f;
		Player.Me().Lock();
		img?.gameObject.SetActive(true);
	}

	// Start is called before the first frame update
    void Start()
    {
		audioPlayer = GetComponent<AudioPlayer>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
		{
			if (fadeIn)
			{
				if (timer <= fadeInCurve.keys[fadeInCurve.keys.Length -1].time)
				{
					ChangeAlpha (timer, fadeInCurve);
					timer += Time.deltaTime;
				}
				else
				{
					timer = 0f;
					fadeIn = false;
					audioPlayer.Play();
				}
			} 
			else if (!audioPlayer.IsPlaying())
			{
				if (timer <= fadeOutCurve.keys[fadeOutCurve.keys.Length -1].time)
				{
					ChangeAlpha (timer, fadeOutCurve);
					timer += Time.deltaTime;
				}
				else
				{
					active = false;
					Player.Me().Unlock();
					img?.gameObject.SetActive(false);
				}
			}

		}
    }

	void ChangeAlpha (float timer, AnimationCurve curve)
	{
		if (img != null)
		{
			Color color = img.color;
			color.a = curve.Evaluate (timer);
			img.color = color;
		}
	}
}

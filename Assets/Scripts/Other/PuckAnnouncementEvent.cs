using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (AudioPlayer))]
public class PuckAnnouncementEvent : MonoBehaviour, IGameEventListener
{
	[SerializeField] GameEvent trigger;
	[SerializeField] float audioDelay;
	[SerializeField] LightController lightController;
	private AudioPlayer audioPlayer;
	bool triggered = false;
	float timer;
	bool playing = false;

	public void OnEventRaised()
	{
		ResetValues();
		triggered = true;
		lightController.EnterAnnouncementState();
	}

    // Start is called before the first frame update
    void Start()
    {
		audioPlayer = GetComponent <AudioPlayer>();
		trigger?.RegisterListener (this);
    }

    // Update is called once per frame
    void Update()
    {
		if (triggered)
		{
			if (!playing)
			{
				if (timer < audioDelay)
					timer += Time.deltaTime;
				else
				{
					audioPlayer.Play();
					playing = true;
					timer = 0f;
				}
			}
			else if (!audioPlayer.IsPlaying())
			{
				if (timer < audioDelay)
					timer += Time.deltaTime;
				else
				{
					lightController.EnterNormalState();
					ResetValues();
				}
			}
		}
    }

	private void OnDestroy()
	{
		trigger?.UnregisterListener (this);
	}

	void ResetValues()
	{
		triggered = false;
		timer = 0f;
		playing = false;
	}
}

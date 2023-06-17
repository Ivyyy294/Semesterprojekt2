using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (AudioPlayer))]
public class AudioEvent : MonoBehaviour, IGameEventListener
{
	[SerializeField] GameEvent trigger;
	[SerializeField] UnityEvent onDone;
	private AudioPlayer audioPlayer;
	bool triggered = false;

	public void OnEventRaised()
	{
		triggered = true;
		audioPlayer.Play();
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
		if (triggered && !audioPlayer.IsPlaying())
		{
			onDone?.Invoke();
			triggered = false;
		}
    }

	private void OnDestroy()
	{
		trigger?.UnregisterListener (this);
	}
}

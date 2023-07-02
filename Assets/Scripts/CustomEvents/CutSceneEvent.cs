using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneEvent : MonoBehaviour
{
	[SerializeField] VideoPlayer videoPlayer;
	[SerializeField] GameEvent creditEvent;

	private void OnEnable()
	{
		videoPlayer.Play();
	}

	// Update is called once per frame
	void Update()
    {
        if (!videoPlayer.isPlaying)
		{
			creditEvent.Raise();
			gameObject.SetActive (false);
		}
    }
}

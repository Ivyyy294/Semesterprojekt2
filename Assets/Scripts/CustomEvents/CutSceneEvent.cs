using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneEvent : MonoBehaviour
{
	[SerializeField] VideoPlayer videoPlayer;
	[SerializeField] GameEvent creditEvent;
	float timer;

	private void OnEnable()
	{
		timer = 0f;
		videoPlayer.Play();
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update()
    {
		if (timer < videoPlayer.length)
			timer += Time.deltaTime;
		else if (!videoPlayer.isPlaying)
		{
			creditEvent.Raise();
			videoPlayer.Stop();
			//gameObject.SetActive (false);
		}
    }
}

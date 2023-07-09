using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneEvent : MonoBehaviour
{
	[System.Serializable]
	struct SubtitleData
	{
		public string text;
		public float timeStamp;
	}

	[SerializeField] VideoPlayer videoPlayer;
	[SerializeField] GameEvent creditEvent;
	[SerializeField] SubtitleData[] subtitles;
	int currentIndex;
	float timer;

	private void OnEnable()
	{
		currentIndex = 0;
		timer = 0f;
		videoPlayer.Play();
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update()
    {
		if (timer < videoPlayer.length)
		{
			if (currentIndex < subtitles.Length)
			{
				if (timer >= subtitles[currentIndex].timeStamp)
					Subtitle.Add (subtitles[currentIndex++].text, (float)videoPlayer.length - timer, 3);
			}

			timer += Time.deltaTime;
		}
		else if (!videoPlayer.isPlaying)
		{
			creditEvent.Raise();
			videoPlayer.Stop();
			//gameObject.SetActive (false);
		}
    }
}

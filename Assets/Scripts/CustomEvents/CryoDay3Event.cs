using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioPlayer))]
class CryoDay3Event : MonoBehaviour
{
	[SerializeField] Door cryoDoor;
	[SerializeField] List <AudioAsset> voiceLines;
	AudioPlayer audioPlayer;
	int currentIndex;
	bool active = false;

	public void Activate()
	{
		active = true;
		currentIndex = 0;
	}


	private void Start()
	{
		active = false;
		audioPlayer = GetComponent <AudioPlayer>();
		currentIndex = 0;
	}

	private void Update()
	{
		if (active)
		{
			cryoDoor.open = true;

			if (currentIndex >= voiceLines.Count && !audioPlayer.IsPlaying())
			{
				active = false;
				cryoDoor.open = false;
			}
			else if (!audioPlayer.IsPlaying())
				audioPlayer.Play(voiceLines[currentIndex++]);
		}
	}
}


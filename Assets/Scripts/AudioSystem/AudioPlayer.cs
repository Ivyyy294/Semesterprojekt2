using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioAsset audioAsset;

	public void Play()
	{
		audioAsset?.Play();
	}

	public void PlayAtPos (Vector3 pos)
	{
		audioAsset?.PlayAtPos(pos);
	}

	public void Stop()
	{
		audioAsset?.Stop();
	}
}

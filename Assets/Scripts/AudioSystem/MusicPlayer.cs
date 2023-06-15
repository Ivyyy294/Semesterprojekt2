using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] MusicAsset musicAsset;

	private void OnEnable()
	{
		musicAsset?.Play();
	}

	private void OnDestroy()
	{
		musicAsset?.Stop();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioAsset audioAsset;

	private void OnEnable()
	{
		audioAsset?.Play();
	}

	private void OnDestroy()
	{
		audioAsset?.Stop();
	}
}

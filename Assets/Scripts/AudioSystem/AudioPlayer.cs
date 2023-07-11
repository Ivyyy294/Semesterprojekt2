using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]  AudioAsset audioAsset;
	[SerializeField] bool playOnAwake = false;
	private AudioSource audioSource;
	private float fadeTime;
	private float baseVolume;
	bool fadeOut = false;

	//Public Functions
	public void Play()
	{
		if (!audioSource.isPlaying)
		{
			audioAsset?.Play(audioSource);
			audioSource.volume = baseVolume;
			fadeOut = false;
		}
	}

	public void Play(AudioAsset newAudioAsset)
	{
		audioAsset = newAudioAsset;

		if (audioSource.isPlaying)
			audioSource.Stop();

		Play();
	}

	public void Stop()
	{
		audioSource.Stop();
	}

	public void FadeOut (float time)
	{
		fadeTime = time;
		fadeOut = true;
	}
	
	public bool IsPlaying () {return audioSource.isPlaying;}

	public AudioAsset AudioAsset() { return audioAsset;}

	//Private Functions
	private void Start()
	{
		audioSource = gameObject.AddComponent (typeof (AudioSource)) as AudioSource;
		baseVolume = audioSource.volume;
		audioSource.playOnAwake = false;
		audioSource.Stop();

		if (playOnAwake)
			Play();
	}

	private void Update()
	{
		if (fadeOut && audioSource.isPlaying)
		{
			if (audioSource.volume > 0f)
				audioSource.volume -= baseVolume * Time.deltaTime / fadeTime;
			else
			{
				audioSource.Stop();
				fadeOut = false;
			}
		}
		else if (audioAsset != null && audioSource.isPlaying)
			audioSource.volume = baseVolume * audioAsset.GetVolumeFactor();
	}

	private void OnDrawGizmosSelected()
	{
		if (audioAsset != null && audioAsset.spatial)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, audioAsset.minDistance);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, audioAsset.maxDistance);
		}
	}
}

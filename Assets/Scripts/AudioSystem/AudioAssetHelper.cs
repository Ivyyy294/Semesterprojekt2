using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAssetHelper : MonoBehaviour
{
	public struct Settings
	{
		public AudioClip clip;
		public string subtitle;
		public AudioAsset.AudioTyp audioTyp;
		public bool loop;
		public float volume;
		public float pitch;
		public bool spatial;
		public float minDistance;
		public float maxDistance;
	}

	public Settings settings;
	private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
		source = gameObject.AddComponent (typeof (AudioSource)) as AudioSource;
    }

	public void Play ()
	{
		if (source == null)
			source = gameObject.AddComponent (typeof (AudioSource)) as AudioSource;

		if (settings.clip != null)
		{
			source.clip = settings.clip;
			source.volume = settings.volume * GetVolumeFactor();
			source.pitch = settings.pitch;
			source.loop = settings.loop;
			source.spatialBlend = settings.spatial ? 1f : 0f;
			source.minDistance = settings.minDistance;
			source.maxDistance = settings.maxDistance;
			source.rolloffMode = AudioRolloffMode.Linear;

			Subtitle.SetText (settings.subtitle);
			source.Play();
		}
	}

	public void Stop()
	{
		source.Stop();
	}

	private void Update()
	{
		source.volume = settings.volume * GetVolumeFactor();
	}


	private float GetVolumeFactor()
	{
		if (settings.audioTyp == AudioAsset.AudioTyp.SFX)
			return AudioSettings.Me().sfxVolume;
		else if (settings.audioTyp == AudioAsset.AudioTyp.MUSIC)
			return AudioSettings.Me().musicVolume;
		else if (settings.audioTyp == AudioAsset.AudioTyp.AMBIENT)
			return AudioSettings.Me().ambientVolume;
		else
			return AudioSettings.Me().uiVolume;
	}
}

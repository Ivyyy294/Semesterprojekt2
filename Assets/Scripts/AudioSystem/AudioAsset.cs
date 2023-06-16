using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu (fileName = "NewAudioAsset", menuName = "AudioAsset")]
public class AudioAsset : ScriptableObject
{
	public enum AudioTyp
	{
		SFX,
		MUSIC,
		AMBIENT,
		UI,
		VOICE_LINE
	}

	public enum PlayStyle
	{
		RANDOM,
		IN_ORDER,
		REVERSE
	}

    public AudioClip[] audioClips;
	public string subtitle;
	[Space]
	[SerializeField] PlayStyle playStyle = PlayStyle.RANDOM;
	[Space]
	public AudioTyp audioTyp = AudioTyp.SFX;
	[HideInInspector] public bool loop = false;
	[HideInInspector] public Vector2 volume = new Vector2 (0.5f, 0.5f);
	[HideInInspector] public Vector2 pitch = new Vector2 (1f, 1f);
	[HideInInspector] public bool spatial = false;
	[HideInInspector] public float minDistance = 0.5f;
	[HideInInspector] public float maxDistance = 500f;

	private Stack <AudioClip> clipBuffer = new Stack <AudioClip>();
	private PlayStyle oldPlayStyle;
	private AudioAssetHelper stableSource;

#if UNITY_EDITOR
	public void PlayPreview()
	{
		//Play preview without spatial
		bool spatialBackup = spatial;
		spatial = false;
		Play (stableSource);
		spatial = spatialBackup;
	}

	public void StopPreview()
	{
		Stop();
	}
#endif

	public void Play (AudioAssetHelper audioSource = null)
	{
		if (audioClips.Length > 0)
		{
			AudioAssetHelper source = IsSFX() ? stableSource : audioSource;

			if (source == null)
				source = CreateAudioSource();

			PlayIntern (source);
		}
	}

	public void PlayAtPos(Vector3 pos, AudioAssetHelper audioSource = null)
	{
		if (audioClips.Length > 0)
		{
			AudioAssetHelper source = IsSFX() ? stableSource : audioSource;

			if (source == null)
				source = CreateAudioSource();

			source.transform.position = pos;
			PlayIntern (source);
		}
	}

	public void Stop()
	{
		stableSource.Stop();
	}

	//Private Functions
	private void OnEnable()
	{
		stableSource = CreateAudioSource();
	}

	private void OnDisable()
	{
		#if UNITY_EDITOR
			DestroyImmediate (stableSource.gameObject);
		#else
			Destroy (stableSource.gameObject);
		#endif
	}

	private bool IsSFX()
	{
		return audioTyp == AudioTyp.MUSIC || audioTyp == AudioTyp.AMBIENT;
	}

	private void PlayIntern(AudioAssetHelper source)
	{
		if (audioClips.Length > 0)
		{
			if (clipBuffer.Count == 0 || oldPlayStyle != playStyle)
				ShuffleAudioClips();

			source.settings = new AudioAssetHelper.Settings()
			{
				clip = clipBuffer.Pop(),
				subtitle = subtitle,
				audioTyp = audioTyp,
				loop = IsSFX() ? false : loop,
				volume = Random.Range (volume.x, volume.y),
				pitch = Random.Range (pitch.x, pitch.y),
				spatial = spatial,
				minDistance = minDistance,
				maxDistance = maxDistance
			};

			source.Play ();

			//Prevents stable source from being deleted
			if (source == stableSource)
				return;

			//Delete tmp audio source after playing
			Destroy (source.gameObject, source.settings.clip.length / source.settings.pitch);
		}

	}

	private void ShuffleAudioClips()
	{
		clipBuffer.Clear();

		if (playStyle == PlayStyle.RANDOM)
		{
			while (clipBuffer.Count < audioClips.Length)
			{
				int index = Random.Range (0, audioClips.Length);

				if (!clipBuffer.Contains (audioClips[index]))
					clipBuffer.Push (audioClips[index]);
			}
		}
		else if (playStyle == PlayStyle.REVERSE)
		{
			foreach (AudioClip i in audioClips)
				clipBuffer.Push (i);
		}
		else
		{
			for (int i = audioClips.Length - 1; i >= 0; --i)
				clipBuffer.Push (audioClips[i]);
		}

		oldPlayStyle = playStyle;
	}

	AudioAssetHelper CreateAudioSource()
	{
		var obj = new GameObject ("AudioAssetSource", typeof (AudioAssetHelper));
		obj.hideFlags = HideFlags.HideAndDontSave;
		return obj.GetComponent <AudioAssetHelper>();
	}
}

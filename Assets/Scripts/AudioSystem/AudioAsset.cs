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
		UI
	}

	public enum PlayStyle
	{
		RANDOM,
		IN_ORDER,
		REVERSE
	}

    public AudioClip[] audioClips;
	[SerializeField] PlayStyle playStyle = PlayStyle.RANDOM;
	[Space]
	public AudioTyp audioTyp = AudioTyp.SFX;
	[HideInInspector] public bool loop = false;
	[HideInInspector] public Vector2 volume = new Vector2 (0.5f, 0.5f);
	[HideInInspector] public Vector2 pitch = new Vector2 (1f, 1f);

	private Stack <AudioClip> clipBuffer = new Stack <AudioClip>();
	private PlayStyle oldPlayStyle;

	private AudioSource stableSource;

#if UNITY_EDITOR
	public void PlayPreview()
	{
		Play (stableSource);
	}

	public void StopPreview()
	{
		Stop();
	}

	private void OnDisable()
	{
			DestroyImmediate (stableSource.gameObject);
	}
#else
	private void OnDisable()
	{
		Destroy (stableSource.gameObject);
	}
#endif

	public void Play (AudioSource audioSource = null)
	{
		if (audioClips.Length > 0)
		{
			AudioSource source = IsSFX() ? stableSource : audioSource;

			if (source == null)
			{
				var obj = new GameObject ("Sound", typeof (AudioSource));
				source = obj.GetComponent <AudioSource>();
			}

			PlayIntern (source);
		}
	}

	public void PlayAtPos(Vector3 pos, AudioSource audioSource = null)
	{
		if (audioClips.Length > 0)
		{
			AudioSource source = IsSFX() ? stableSource : audioSource;

			if (source == null)
			{
				var obj = new GameObject ("Sound", typeof (AudioSource));
				source = obj.GetComponent <AudioSource>();
			}

			source.transform.position = pos;
			PlayIntern (source);
		}
	}

	public void Stop()
	{
		stableSource.Stop();
	}

	//Private Functions
	private bool IsSFX()
	{
		return audioTyp == AudioTyp.MUSIC || audioTyp == AudioTyp.AMBIENT;
	}

	private void PlayIntern(AudioSource source)
	{
		if (audioClips.Length > 0)
		{
			if (clipBuffer.Count == 0 || oldPlayStyle != playStyle)
				ShuffleAudioClips();

			source.clip = clipBuffer.Pop();
			source.volume = Random.Range (volume.x, volume.y);
			source.pitch = Random.Range (pitch.x, pitch.y);
			source.loop = IsSFX() ? false : loop;
			source.Play();

			//Prevents stable source from being deleted
			if (source == stableSource)
				return;

			//Delete tmp audio source after playing
			Destroy (source.gameObject, source.clip.length / source.pitch);
		}

	}

	private void OnEnable()
	{
		stableSource = EditorUtility.CreateGameObjectWithHideFlags ("AudioPreview", HideFlags.HideAndDontSave, typeof (AudioSource)).GetComponent <AudioSource>();
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
}

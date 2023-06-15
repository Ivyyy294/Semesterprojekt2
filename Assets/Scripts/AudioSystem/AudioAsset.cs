using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu (fileName = "NewSoundEffectAsset", menuName = "SoundEffectAsset")]
public class AudioAsset : ScriptableObject
{
	public enum PlayStyle
	{
		RANDOM,
		IN_ORDER,
		REVERSE
	}

    public AudioClip[] audioClips;
	[HideInInspector] public Vector2 volume = new Vector2 (0.5f, 0.5f);
	[HideInInspector] public Vector2 pitch = new Vector2 (1f, 1f);
	[SerializeField] PlayStyle playStyle = PlayStyle.RANDOM;

	private Stack <AudioClip> clipBuffer = new Stack <AudioClip>();
	private PlayStyle oldPlayStyle;

#if UNITY_EDITOR
	private AudioSource previewer;

	private void OnEnable()
	{
		previewer = EditorUtility.CreateGameObjectWithHideFlags ("AudioPreview", HideFlags.HideAndDontSave, typeof (AudioSource)).GetComponent <AudioSource>();
	}

	private void OnDisable()
	{
		DestroyImmediate (previewer.gameObject);
	}

	public void PlayPreview()
	{
		Play (previewer);
	}

	public void StopPreview()
	{
		previewer.Stop();
	}
#endif

	public void Play (AudioSource audioSource = null)
	{
		if (audioClips.Length > 0)
		{
			if (clipBuffer.Count == 0 || oldPlayStyle != playStyle)
				ShuffleAudioClips();

			AudioSource source = audioSource;

			if (source == null)
			{
				var obj = new GameObject ("Sound", typeof (AudioSource));
				source = obj.GetComponent <AudioSource>();
			}

			source.clip = clipBuffer.Pop();
			source.volume = Random.Range (volume.x, volume.y);
			source.pitch = Random.Range (pitch.x, pitch.y);
			source.Play();

#if UNITY_EDITOR
			if (audioSource == previewer)
				return;
#endif

			Destroy (source.gameObject, source.clip.length / source.pitch);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu (fileName = "NewMusicAsset", menuName = "MusicAsset")]
public class MusicAsset : ScriptableObject
{
    public AudioClip musicClip;
	[Range (0f, 1f)]
	[SerializeField] float volume = 0.5f;
	[Range (0f, 2f)]
	[SerializeField] float pitch = 1f;
	[SerializeField] bool loop = false;

	private AudioSource source;

	private void OnEnable()
	{
		source = EditorUtility.CreateGameObjectWithHideFlags ("AudioPreview", HideFlags.HideAndDontSave, typeof (AudioSource)).GetComponent <AudioSource>();
	}

	private void OnDisable()
	{
		#if UNITY_EDITOR
			DestroyImmediate (source.gameObject);
		#else
			Destroy (source.gameObject);
		#endif
	}

	public void Stop()
	{
		source.Stop();
	}

	public void Play ()
	{
		if (musicClip != null)
		{
			source.clip = musicClip;
			source.volume = volume;
			source.pitch = pitch;
			source.loop = loop;
			source.Play();
		}
	}
}

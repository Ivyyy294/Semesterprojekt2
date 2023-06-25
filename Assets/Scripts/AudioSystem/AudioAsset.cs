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

#if UNITY_EDITOR
	private AudioSource preview;

	private void OnEnable()
	{
		preview = CreateAudioSource();
	}

	private void OnDisable()
	{
		DestroyImmediate (preview);
	}

	public void PlayPreview()
	{
		//Play preview without spatial
		Play (preview).spatialBlend = 0f;;
	}

	public void StopPreview()
	{
		preview.Stop();
	}
#endif

	public void PlayOneShot()
	{
		Play (null);
	}

	public AudioSource Play (AudioSource audioSource = null)
	{
		AudioSource source = audioSource;

	
		if (audioClips.Length > 0)
		{
			if (clipBuffer.Count == 0 || oldPlayStyle != playStyle)
				ShuffleAudioClips();

			AudioClip clip = clipBuffer.Pop();

			if (clip != null)
			{
				if (source == null)
					source = CreateAudioSource();

				source.clip = clip;
				//Only Allow loop with externen AudioSource
				source.loop = audioSource != null && loop;
				source.volume = Random.Range (volume.x, volume.y) * GetVolumeFactor();
				source.pitch = Random.Range (pitch.x, pitch.y);
				source.spatialBlend = spatial ? 1f : 0f;
				source.minDistance = minDistance;
				source.maxDistance = maxDistance;
				source.rolloffMode = AudioRolloffMode.Linear;
				source.Play ();

	#if UNITY_EDITOR
				//Prevents stable source from being deleted
				if (audioSource == preview)
					return source;
	#endif
				//Delete tmp audio source after playing
				if (audioSource == null)
					Destroy (source.gameObject, source.clip.length / source.pitch);
			}
			else
				Debug.LogError("Invalid CLip!");
		}
		//Shows the Subtitle even when clip is null as a placeholder

		if (audioTyp != AudioTyp.MUSIC && audioTyp != AudioTyp.AMBIENT && source != null && source.clip != null)
			ShowSubtitle (source.clip.length / source.pitch);
		else 
			ShowSubtitle ();


		return source;
	}

	public void PlayAtPos(Vector3 pos)
	{
		AudioSource tmp = Play ();

		if (tmp!= null)
			tmp.transform.position = pos;
	}

	public float GetVolumeFactor()
	{
		if (audioTyp == AudioAsset.AudioTyp.SFX)
			return AudioSettings.Me().sfxVolume;
		else if (audioTyp == AudioAsset.AudioTyp.MUSIC)
			return AudioSettings.Me().musicVolume;
		else if (audioTyp == AudioAsset.AudioTyp.AMBIENT)
			return AudioSettings.Me().ambientVolume;
		else
			return AudioSettings.Me().uiVolume;
	}

	//Private FUnctions
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

	AudioSource CreateAudioSource()
	{
		var obj = new GameObject ("AudioAssetSource", typeof (AudioSource));
		obj.hideFlags = HideFlags.HideAndDontSave;
		return obj.GetComponent <AudioSource>();
	}

	void ShowSubtitle (float playTime = 0f)
	{
		float minTime = 0.75f;
		//Making sure the subtile is readable for at lest 1 second
		playTime = Mathf.Max (minTime, playTime);

		int priority = 0;

		if (audioTyp == AudioTyp.VOICE_LINE)
			priority = 2;
		if (audioTyp != AudioTyp.AMBIENT)
			priority = 1;

		Subtitle.Add (subtitle, playTime, priority);
	}
}

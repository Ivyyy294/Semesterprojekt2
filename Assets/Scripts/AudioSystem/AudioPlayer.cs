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

	public void PlayAtPos ()
	{
		audioAsset?.PlayAtPos(transform.position);
	}

	public void PlayAtPos (Vector3 pos)
	{
		audioAsset?.PlayAtPos(pos);
	}

	public void Stop()
	{
		audioAsset?.Stop();
	}

	private void OnDrawGizmos()
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

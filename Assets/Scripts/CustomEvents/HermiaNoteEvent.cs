using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Animator))]
public class HermiaNoteEvent : MonoBehaviour
{
	[SerializeField] AudioPlayer audioPlayer;
	[SerializeField] Animator animator;

	public void PlayAudio()
	{
		audioPlayer.Play();
	}

	public void StartAanimation()
	{
		animator.SetTrigger("Start");
	}
}

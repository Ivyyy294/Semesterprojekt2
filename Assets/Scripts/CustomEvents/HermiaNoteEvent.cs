using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Animator))]
public class HermiaNoteEvent : MonoBehaviour
{
	[SerializeField] string propertyName;
	[SerializeField] AudioPlayer audioPlayer;
	[SerializeField] Animator animator;

	public void PlayAudio()
	{
		audioPlayer.Play();
	}

	public void AnimationDone()
	{
		string guid = BlackBoard.Me().GetGuidByName (propertyName);
		BlackBoard.Me().EditValue (guid, BlackBoard.EditTyp.SET, 1);
	}

	public void StartAanimation()
	{
		animator.SetTrigger("Start");
	}
}

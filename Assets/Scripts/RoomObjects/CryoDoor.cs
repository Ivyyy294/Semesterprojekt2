using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class CryoDoor : MonoBehaviour
{
	[SerializeField] AudioPlayer audioPlayer;
	Animator animator;
	bool open = false;
	public bool Open => open;

	public void SpawnOpen()
	{
		open = true;
		animator.SetTrigger ("SpawnOpen");
	}

	public void SetOpen(bool val)
	{
		open = val;

		if (open)
			animator?.SetTrigger("Open");
		else
			animator?.SetTrigger ("Close");
	}

	public void PlaySound()
	{
		if (audioPlayer != null)
			audioPlayer.Play();
	}

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent <Animator>();

		if (animator == null)
			Debug.LogError ("Missing Animator!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

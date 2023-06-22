using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.StateMachine;

public class LightController : MonoBehaviour
{
	public Animator animator;

	//Public Functions
	public void EnterNormalState()
	{
		animator.SetTrigger("Normal");
	}

	public void EnterAnnouncementState()
	{
		animator.SetTrigger("Announcement");
	}

	//Private Functions
    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent <Animator>();
    }
}

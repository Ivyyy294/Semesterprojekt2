using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckInterface : MonoBehaviour
{
	[SerializeField] Animator animator;

	//Protected Functions
	public void OnEmotionChanged (Puck.Emotion emotion)
	{
		if (animator == null)
			Debug.LogError("Missing Animator!");
		else
			animator.SetTrigger (emotion.ToString());
	}

	//Private Functions
	private void Start()
	{
		animator = GetComponent <Animator>();
	}

	private void OnEnable()
	{
		Puck.RegisterInterface (this);
	}

	private void OnDisable()
	{
		Puck.DeregisterInterface (this);
	}
}

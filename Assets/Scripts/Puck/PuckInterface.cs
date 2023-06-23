using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckInterface : MonoBehaviour
{
	[SerializeField] Animator animator;

	//Protected Functions
	public void OnEmotionChanged (Puck.Emotion emotion)
	{
		if (animator = null)
			animator.SetTrigger (emotion.ToString());
		else
			Debug.LogError("Missing Animator!");
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

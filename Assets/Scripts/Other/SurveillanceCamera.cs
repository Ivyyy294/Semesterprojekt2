using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ivyyy.Interfaces;

public class SurveillanceCamera : MonoBehaviour
{
	[SerializeField] float speed = 20f;
	[SerializeField] Transform rayOrigin;
	[SerializeField] float minPlayerLostTime;
	[SerializeField] AudioAsset playerFound;

	private Transform playerTransform;
	float timerPlayerLost = 0f;

	enum State
	{
		IDLE,
		TRACKING
	}

	private Vector3 resetPos;
	State currentState = State.IDLE;

	private void Start()
	{
		playerTransform = Camera.main.transform.parent;
		resetPos = transform.forward;
	}

	// Update is called once per frame
	void Update()
    {
		if (currentState == State.IDLE)
			Idle();
		else
			Tracking();
    }

	void Rotate (Vector3 target)
	{
		transform.forward = Vector3.MoveTowards (transform.forward, target, speed * Time.deltaTime);
	}

	void Tracking()
	{
		Vector3 targetDir = playerTransform.position - transform.position;

		Rotate (targetDir);

		if (!Search())
			currentState = State.IDLE;
	}

	void Idle()
	{
        if (Search())
		{
			if (timerPlayerLost >= minPlayerLostTime)
				playerFound?.PlayAtPos(transform.position);

			timerPlayerLost = 0f;
			currentState = State.TRACKING;
		}
		else
			timerPlayerLost += Time.deltaTime;
	}

	bool Search ()
	{
		bool hit = rayOrigin.GetComponent<CameraSensor>().Scan(Camera.main.transform.position);
		
		if (!hit)
			hit = rayOrigin.GetComponent<CameraSensor>().Scan(playerTransform.position);

		return hit;
	}
}
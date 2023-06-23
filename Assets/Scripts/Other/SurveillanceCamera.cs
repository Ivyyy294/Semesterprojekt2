using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ivyyy.Interfaces;

public class SurveillanceCamera : MonoBehaviour
{
	[SerializeField] float speed = 20f;
	[SerializeField] float range = 50f;
	[SerializeField] Transform rayOrigin;
	private Transform playerTransform;

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
			currentState = State.TRACKING;
	}

	bool Search ()
	{
		Vector3 direction = playerTransform.position - rayOrigin.position;
		Ray ray = new Ray (rayOrigin.position, direction);

		RaycastHit hit;

		bool inRange = false;

		if (Physics.Raycast(ray, out hit, range))
			inRange = hit.collider.CompareTag ("Player");

		Debug.DrawRay (ray.origin, ray.direction * range, inRange ? Color.green : Color.red);

		return inRange;
	}
}

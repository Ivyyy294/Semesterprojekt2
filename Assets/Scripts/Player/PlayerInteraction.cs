using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;
using Ivyyy.GameEvent;

public class PlayerInteraction : MonoBehaviour
{
	[SerializeField] float range;
	[SerializeField] KeyCode interactKey;
	[SerializeField] KeyCode pauseKey;
	
	[Header ("Lara Values")]
	[SerializeField] GameEvent showPauseMenu;
	[SerializeField] InteractTextOverlay interactTextOverlay;

	private Transform cameraTrans;

	//Private
	private void Start()
	{
		cameraTrans = Camera.main.transform;

		if (interactTextOverlay == null)
			Debug.LogError ("Missing InteractTextOverlay reference!");
	}

	// Update is called once per frame
	void Update()
    {
		bool showInteractOverlay = false;
		//Clears the text overlay
		if (Time.timeScale > 0f)
		{
			InteractableObject tmp;

			if (InteractableObjectInSight (out tmp))
			{
				if (Input.GetKeyDown (interactKey))
					tmp.Interact();
				else
					showInteractOverlay = true;
			}

			if (Input.GetKeyDown (KeyCode.Escape))
				showPauseMenu?.Raise();
		}

		interactTextOverlay?.Show(showInteractOverlay);
    }

	bool InteractableObjectInSight (out InteractableObject interactableObject)
	{
		Ray ray = new Ray (cameraTrans.position, cameraTrans.forward);

		RaycastHit hit;

		bool inRange = false;

		if (Physics.Raycast(ray, out hit, range))
		{
			interactableObject = hit.transform.gameObject.GetComponent<InteractableObject>();
			
			if (interactableObject != null && ((MonoBehaviour)interactableObject).enabled)
				inRange = true;
		}
		else
			interactableObject = null;

		Debug.DrawRay (ray.origin, ray.direction * range, inRange ? Color.green : Color.red);

		return inRange;
	}
}

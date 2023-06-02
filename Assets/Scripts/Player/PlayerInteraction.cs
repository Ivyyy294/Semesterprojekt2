using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Interfaces;

public class PlayerInteraction : MonoBehaviour
{
	[SerializeField] float range;
	[SerializeField] KeyCode interactKey;
	//[SerializeField] PauseMenu pauseMenu;

	private Transform cameraTrans;

	//Private
	private void Start()
	{
		cameraTrans = Camera.main.transform;
	}

	// Update is called once per frame
	void Update()
    {
		if (Time.timeScale > 0f)
		{
			if (Input.GetKeyDown (interactKey))
				Interact();

			//if (Input.GetKeyDown (KeyCode.Escape))
			//	pauseMenu.gameObject.SetActive (true);
		}
    }

	bool Interact()
	{
		Ray ray = new Ray (cameraTrans.position, cameraTrans.forward);

		RaycastHit hit;

		bool inRange = false;

		if (Physics.Raycast(ray, out hit, range))
		{
			InteractableObject tmp = hit.transform.gameObject.GetComponent<InteractableObject>();
			
			if (tmp != null)
			{
				tmp.Interact();
				inRange = true;
			}
		}

		Debug.DrawRay (ray.origin, ray.direction * range, inRange ? Color.green : Color.red);

		return inRange;
	}
}

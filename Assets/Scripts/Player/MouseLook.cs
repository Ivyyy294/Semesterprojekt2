using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MouseLook : MonoBehaviour
{
	float verticalRotation;
	float horizontalRotation;
	public Transform cameraTrans;
	[SerializeField] float maxVerticalAngle = 80f;
	[SerializeField] bool limitHorizontalAngle = false;
	[SerializeField] float maxHorizontalAngle = 0f;

	[Range (0.1f, 2f)]
	[SerializeField] float mouseSensitivity = 1f;

	public float GetRotationX() { return verticalRotation;}
	public float GetRotationY() { return transform.rotation.eulerAngles.y;}

	// Start is called before the first frame update
	void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
    }

	private void OnEnable()
	{
		verticalRotation = -cameraTrans.localEulerAngles.x;
		horizontalRotation = 0f;
	}

	// Update is called once per frame
	void Update()
    {
		if (Time.timeScale > 0f)
		{
			MoveHorizontal();
			MoveVertical();
		}
    }

	void MoveHorizontal() 
	{
		float mouseX = Input.GetAxis ("Mouse X") * mouseSensitivity;
		horizontalRotation += mouseX;

		if (limitHorizontalAngle)
		{
			Debug.Log (horizontalRotation);
			float offset = 0f;
			if(horizontalRotation > maxHorizontalAngle)
				offset = maxHorizontalAngle - horizontalRotation;
			else if (horizontalRotation < -maxHorizontalAngle)
				offset = -maxHorizontalAngle - horizontalRotation; 

			Debug.Log (offset);

			if (offset != 0f)
			{
				mouseX += offset;
				horizontalRotation += offset;
			}
		}

		transform.Rotate (Vector3.up, mouseX);
	}

	void MoveVertical()
	{
		verticalRotation += Input.GetAxis ("Mouse Y")  * mouseSensitivity;
		verticalRotation = Mathf.Clamp (verticalRotation, -maxVerticalAngle, maxVerticalAngle);

		SetRotationX (verticalRotation);
	}

	public void SetRotationX (float val)
	{
		if (val != verticalRotation)
			verticalRotation = val;

		cameraTrans.localRotation = Quaternion.Euler (new Vector3 (-val, 0f, 0f));
	}

	void SetRotationY (float val)
	{
		transform.rotation = Quaternion.Euler (0f, val, 0f);
	}
}

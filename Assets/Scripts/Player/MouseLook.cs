using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MouseLook : MonoBehaviour
{
	float verticalRotation;
	[SerializeField] Transform cameraTrans;
	[SerializeField] float maxVerticalAngle = 80f;

	[Range (0.1f, 2f)]
	[SerializeField] float mouseSensitivity = 1f;

	public float GetRotationX() { return verticalRotation;}
	public float GetRotationY() { return transform.rotation.eulerAngles.y;}

	public void ResetRotation()
	{
		verticalRotation = 0f;
		SetRotationX (0f);
		SetRotationX (0f);
	}

    // Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
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
		cameraTrans.localRotation = Quaternion.Euler (new Vector3 (-val, 0f, 0f));
	}

	public void SetRotationY (float val)
	{
		transform.rotation = Quaternion.Euler (0f, val, 0f);
	}
}

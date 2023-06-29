using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MouseLook : MonoBehaviour
{
	float verticalRotation;
	float defaultPov;
	[SerializeField] Transform cameraTrans;
	[SerializeField] float maxVerticalAngle = 80f;
	[SerializeField] float zoomPov;
	[SerializeField] float zoomSpeed;

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
		defaultPov = Camera.main.fieldOfView;
		Cursor.lockState = CursorLockMode.Locked;
		//cameraTrans = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
		if (Time.timeScale > 0f)
		{
			MoveHorizontal();
			MoveVertical();

			CinemachineVirtualCamera vCam = (CinemachineVirtualCamera) Player.Me().cinemachineBrain.ActiveVirtualCamera;
			float dist = defaultPov - zoomPov;

			if (Input.GetKey (KeyCode.Mouse1))
				vCam.m_Lens.FieldOfView = Mathf.MoveTowards (vCam.m_Lens.FieldOfView, zoomPov, dist * Time.deltaTime * zoomSpeed);
			else
				vCam.m_Lens.FieldOfView = Mathf.MoveTowards (vCam.m_Lens.FieldOfView, defaultPov, dist * Time.deltaTime * zoomSpeed);
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
		//cameraTrans.Rotate (Vector3.right, -mouseY, Space.Self);
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

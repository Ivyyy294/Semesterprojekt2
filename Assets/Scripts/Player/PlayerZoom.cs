using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerZoom : MonoBehaviour
{
	[SerializeField] float zoomPov;
	[SerializeField] float zoomSpeed;
	[SerializeField] CinemachineVirtualCamera vCam;
	float defaultPov;

    // Start is called before the first frame update
    void Start()
    {
		defaultPov = Camera.main.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {       
		float dist = defaultPov - zoomPov;
		
		if (!Player.Me().cinemachineBrain.IsBlending
			&& (CinemachineVirtualCamera)Player.Me().cinemachineBrain.ActiveVirtualCamera == vCam
			&& Input.GetKey (KeyCode.Mouse1))
				vCam.m_Lens.FieldOfView = Mathf.MoveTowards (vCam.m_Lens.FieldOfView, zoomPov, dist * Time.deltaTime * zoomSpeed);
		else
			vCam.m_Lens.FieldOfView = Mathf.MoveTowards (vCam.m_Lens.FieldOfView, defaultPov, dist * Time.deltaTime * zoomSpeed);
    }
}

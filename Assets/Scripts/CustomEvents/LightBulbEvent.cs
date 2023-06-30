using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioPlayer))]
public class LightBulbEvent : MonoBehaviour
{
	[SerializeField] GameObject lightBulb;
	AudioPlayer audioPlayer;

	public void DisableLight(bool silent)
	{
		lightBulb.SetActive (false);
		
		if (!silent)
			audioPlayer.Play();
	}

    // Start is called before the first frame update
    void Start()
    {
		audioPlayer = GetComponent <AudioPlayer>();
    }
}

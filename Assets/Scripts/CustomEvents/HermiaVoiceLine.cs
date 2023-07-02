using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

[RequireComponent (typeof(AudioPlayer))]
public class HermiaVoiceLine : MonoBehaviour
{
	[SerializeField] string propertyName;
	AudioPlayer audioPlayer;
	bool active;
    // Start is called before the first frame update
    void Start()
    {
		audioPlayer = GetComponent<AudioPlayer>();
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active && !audioPlayer.IsPlaying())
		{
			BlackBoard.Me().EditValue (BlackBoard.Me().GetGuidByName(propertyName), BlackBoard.EditTyp.SET, 1);
			gameObject.SetActive(false);
		}
    }

	public bool IsActive() {return active;}

	public void Activate()
	{
		active = true;
		audioPlayer.Play();
	}

}

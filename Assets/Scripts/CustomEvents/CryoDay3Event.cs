using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioPlayer))]
class CryoDay3Event : MonoBehaviour
{
	[SerializeField] Door cryoDoor;
	[SerializeField] InteractableCamera cryoChair;
	[SerializeField] Room room;
	[SerializeField] List <AudioAsset> voiceLines;
	[SerializeField] AudioAsset voiceLinesTrapped;
	[SerializeField] BlackBoardProperty testProperty;
	[SerializeField] ChatTerminalObj chatTerminal;

	AudioPlayer audioPlayer;
	int currentIndex;
	bool active = false;
	bool playerInCryoRoom = false;

	public void Activate()
	{
		active = true;
		playerInCryoRoom = false;
		currentIndex = 0;
		cryoDoor.open = true;
		chatTerminal.SetLocked(true);
	}

	public void SetPlayerInCryoRoom()
	{
		playerInCryoRoom = true;
	}

	private void Start()
	{
		active = false;
		audioPlayer = GetComponent <AudioPlayer>();
		currentIndex = 0;
	}

	private void Update()
	{
		if (active)
		{
			if (playerInCryoRoom && cryoDoor.open)
			{
				Debug.Log("close door");
				audioPlayer.Play (voiceLinesTrapped);
				cryoDoor.open = false;
				chatTerminal.SetLocked(false);
			}
			else if (playerInCryoRoom && cryoChair.IsActive())
			{
				active = false;
				chatTerminal.SetLocked(false);

				BlackBoardProperty checkValue = BlackBoard.Me().GetPropertyByName (testProperty.name);

				if (checkValue.Compare (testProperty))
					room.EnterEndingCryoGood();
				else
					room.EnterEndingCryoBad();
			}
			else if (!audioPlayer.IsPlaying())
				audioPlayer.Play(voiceLines[currentIndex++]);
		}
	}
}


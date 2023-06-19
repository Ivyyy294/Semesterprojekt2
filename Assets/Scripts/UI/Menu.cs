using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] GameEvent newGame;
	[SerializeField] GameEvent continueGame;
	[SerializeField] GameEvent closeGame;
	[SerializeField] GameObject continueButton;


	public void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
		continueButton.GetComponent<Button>().interactable = SaveGameManager.Me().SaveGameAvailable();
	}

	public void OnNewGameButton()
	{
		BlackBoard.Me().Clear();
		newGame?.Raise();
	}

	public void OnContinueButton()
	{
		BlackBoard.Me().Clear();
		continueGame?.Raise();
	}

    public void OnExitButton()
	{
		closeGame?.Raise();
	}
}

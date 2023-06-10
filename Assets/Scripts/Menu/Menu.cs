using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] GameEvent newGame;
	[SerializeField] GameEvent continueGame;
	[SerializeField] GameEvent closeGame;

	public void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
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

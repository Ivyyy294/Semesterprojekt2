using UnityEngine;

public class SaveGameHandler : MonoBehaviour
{
    public void LoadGame()
	{
		Debug.Log ("Loading...");
		Ivyyy.SaveGameSystem.SaveGameManager.Me().LoadGameState();
	}

	public void SaveGame()
	{
		Debug.Log ("Saving...");
		Ivyyy.SaveGameSystem.SaveGameManager.Me().SaveGameState();
	}
}

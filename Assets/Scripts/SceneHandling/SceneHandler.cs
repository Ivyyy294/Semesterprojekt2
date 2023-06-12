using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ivyyy.SaveGameSystem;

public class SceneHandler : MonoBehaviour
{
	[Header ("Lara Values")]
	[SerializeField] int indexSplashScene = 1;
	[SerializeField] int indexMenuScene = 2;
	[SerializeField] int indexGameScene = 3;
	public bool loadGame = false;

	int currentSceneIndex = -1;

	public void LoadMenu()
	{
		SwitchScene (indexMenuScene);
	}

	public void LoadGame (bool newGame = false)
	{
		SwitchScene (indexGameScene);

		if (!newGame)
			SaveGameManager.Me().ScheduledLoadGame();
	}

	public void CloseGame()
	{
		Application.Quit();
	}

    // Start is called before the first frame update
    void Start()
    {
		SwitchScene (indexSplashScene);
    }

	void SwitchScene (int newIndex)
	{
		List <int> loadedScenes = GetLoadedScenes();

		if (!loadedScenes.Contains (newIndex))
		{
			currentSceneIndex = newIndex;

			//Unload all scenes but root
			foreach (int i in loadedScenes)
			{
				if (i != 0)
					SceneManager.UnloadSceneAsync (i);
			}

			SceneManager.LoadScene (currentSceneIndex, LoadSceneMode.Additive);
		}
	}

	List<int> GetLoadedScenes ()
	{
		List<int> loadedScenes = new List<int>();

		for (int i = 0; i < SceneManager.sceneCount; ++i)
			loadedScenes.Add (SceneManager.GetSceneAt (i).buildIndex);

		return loadedScenes;
	} 

}

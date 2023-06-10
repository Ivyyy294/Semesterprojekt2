using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ivyyy.SaveGameSystem;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AdditiveSceneLoader : MonoBehaviour
{
	[SerializeField] List <string> scenesToLoad;

	#if UNITY_EDITOR
		[SerializeField] List <SceneAsset> sceneAssets;
	#endif

	private List <AsyncOperation> sceneStatus = new List <AsyncOperation>();

    // Start is called before the first frame update
    void Start()
    {
		List <string> loadedScenes = new List<string>();

		for (int i = 0; i < SceneManager.sceneCount; ++i)
			loadedScenes.Add (SceneManager.GetSceneAt (i).name);

		foreach (string i in scenesToLoad)
		{
			if (!loadedScenes.Contains (i))
				sceneStatus.Add (SceneManager.LoadSceneAsync (i, LoadSceneMode.Additive));
		}
    }

	private void Update()
	{
		//Checks if a load game is schedualed and does so after all scenes are done loading
		if (SaveGameManager.Me().LoadGameScheduled)
		{
			if (sceneStatus.Any(x=>x.isDone == false))
				return;

			SaveGameManager.Me().LoadGameState();
		}
	}
}

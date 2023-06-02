using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{
	[SerializeField] SceneAsset[] scenesToLoad;

    // Start is called before the first frame update
    void Start()
    {
		List <string> loadedScenes = new List<string>();

		for (int i = 0; i < SceneManager.sceneCount; ++i)
			loadedScenes.Add (SceneManager.GetSceneAt (i).name);

		foreach (SceneAsset i in scenesToLoad)
		{
			if (!loadedScenes.Contains (i.name))
				SceneManager.LoadScene (i.name, LoadSceneMode.Additive);
		}
    }
}

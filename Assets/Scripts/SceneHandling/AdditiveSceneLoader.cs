using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class AdditiveSceneLoader : MonoBehaviour
{
	[SerializeField] List <string> scenesToLoad;

	#if UNITY_EDITOR
		[SerializeField] List <SceneAsset> sceneAssets;
	#endif
    // Start is called before the first frame update
    void Start()
    {
		List <string> loadedScenes = new List<string>();

		for (int i = 0; i < SceneManager.sceneCount; ++i)
			loadedScenes.Add (SceneManager.GetSceneAt (i).name);

		foreach (string i in scenesToLoad)
		{
			if (!loadedScenes.Contains (i))
				SceneManager.LoadScene (i, LoadSceneMode.Additive);
		}
    }
}

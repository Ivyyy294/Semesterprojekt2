using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.SaveGameSystem;

public class SaveGameLoader : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
		if (SaveGameManager.Me().LoadGameScheduled)
			SaveGameManager.Me().LoadGameState();
    }
}

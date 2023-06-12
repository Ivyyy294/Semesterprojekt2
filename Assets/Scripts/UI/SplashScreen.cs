using Ivyyy.GameEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
	[SerializeField] GameEvent showMenuGameEvent;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
			showMenuGameEvent?.Raise();
    }
}

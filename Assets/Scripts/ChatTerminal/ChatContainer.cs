using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatContainer : MonoBehaviour
{
	[SerializeField] GameObject[] chatContainer;
    int currentIndex = 0;

	public void SetActiveChat (int index)
	{
		if (index >= 0 && index < chatContainer.Length)
		{
			if (currentIndex != index)
			{
				chatContainer[currentIndex].SetActive (false);
				chatContainer[index].SetActive (true);
				currentIndex = index;
			}
		}
		else
			Debug.LogError ("Invalid Error!");
	}
}

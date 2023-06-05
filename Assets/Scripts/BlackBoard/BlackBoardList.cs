using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu (menuName = "BlackBoardList")]
public class BlackBoardList : ScriptableObject
{
    public List <string> propertyList = new List<string>();
}

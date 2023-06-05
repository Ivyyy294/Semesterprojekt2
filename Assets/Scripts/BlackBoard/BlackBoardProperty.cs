using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlackBoardProperty
{
	public enum ComparisonTyp
	{
		EQUAL,
		GREATER,
		LESS
	}

	public string name;
	public ComparisonTyp comparisonTyp;
	public int iVal;
}

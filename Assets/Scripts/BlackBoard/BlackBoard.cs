using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoard
{
	[System.Serializable]
	public enum EditTyp
	{
		SET,
		INCREASE,
		DECREASE
	}

	private static BlackBoard me;
	private List <BlackBoardProperty> properties = new List <BlackBoardProperty>();
	private BlackBoard(){}

	public static BlackBoard Me ()
	{
		if (me == null)
			me = new BlackBoard();

		return me;
	}

	public void Clear()
	{
		properties.Clear();
	}

	public void EditValue (string name, EditTyp editTyp, int val)
	{
		BlackBoardProperty tmp = GetProperty (name);

		if (editTyp == EditTyp.SET)
			tmp.iVal = val;
		else if (editTyp == EditTyp.INCREASE)
			tmp.iVal += val;
		else
			tmp.iVal -= val;
	}

	public BlackBoardProperty GetProperty (string name)
	{
		foreach (BlackBoardProperty i in properties)
		{
			if (i.name == name)
				return i;
		}

		BlackBoardProperty tmp = new BlackBoardProperty() {name = name};
		properties.Add (tmp);

		return tmp;
	}
}

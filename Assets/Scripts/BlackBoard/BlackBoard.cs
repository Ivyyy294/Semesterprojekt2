using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoard
{
	private static BlackBoard me;

	private List <BlackBoardProperty> properties = new List <BlackBoardProperty>();
	private BlackBoard(){}

	public static BlackBoard Me()
	{
		if (me == null)
			me = new BlackBoard();

		return me;
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

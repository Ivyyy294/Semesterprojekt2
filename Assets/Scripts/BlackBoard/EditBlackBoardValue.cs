using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.GameEvent;

public class EditBlackBoardValue : MonoBehaviour
{
	public BlackBoardList blackBoardList;
	[SerializeField] string propertyName;
	[SerializeField] BlackBoard.EditTyp editTyp;
	[SerializeField] int value;

	public void ModifyValue()
	{
		BlackBoard.Me().EditValue (propertyName, editTyp, value);
	}
}

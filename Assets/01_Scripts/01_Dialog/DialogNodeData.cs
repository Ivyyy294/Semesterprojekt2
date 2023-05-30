using UnityEngine;

[System.Serializable]
public class DialogNodeData
{
	public enum NodeType
	{
		MultipleChoice,
		GameEvent
	}

	public string Guid;
	public string DialogTitle;
	public string DialogText;
	public Vector2 Position;
	public NodeType Type = NodeType.MultipleChoice;
}
 
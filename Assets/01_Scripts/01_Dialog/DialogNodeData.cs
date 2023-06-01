using UnityEngine;
using Ivyyy.GameEvent;

[System.Serializable]
public class DialogNodeData
{
	public enum NodeType
	{
		MultipleChoice,
		Auto,
		GameEvent
	}

	public string Guid;
	public string DialogTitle;
	public string DialogText;
	public Vector2 Position;
	public NodeType Type = NodeType.MultipleChoice;
	public GameEvent GameEvent = null;
}
 
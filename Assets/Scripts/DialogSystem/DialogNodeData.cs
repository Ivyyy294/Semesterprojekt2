using UnityEngine;
using Ivyyy.GameEvent;

[System.Serializable]
public class DialogNodeData
{
	public enum NodeType
	{
		CHOICE,
		NPC,
		RAISE_EVENT,
		LISTEN_EVENT
	}

	//Base Values
	public string Guid;
	public string DialogTitle;
	public Vector2 Position;
	public NodeType Type = NodeType.CHOICE;

	//Text Node Values
	public string DialogText;
	public Sprite Image;

	//Event Node Values
	public GameEvent GameEvent = null;
}

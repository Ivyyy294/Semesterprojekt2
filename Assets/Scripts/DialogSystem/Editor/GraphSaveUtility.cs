using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
	private DialogGraphView targetGraphView;
	private DialogContainer containerCache;

	private List <Edge> Edges => targetGraphView.edges.ToList();
	private List <DialogNode> Nodes => targetGraphView.nodes.ToList().Cast<DialogNode>().ToList();

	public static GraphSaveUtility GetInstance (DialogGraphView target)
	{
		return new GraphSaveUtility {targetGraphView = target};
	}

	public void SaveGraph (DialogContainer asset)
	{
		Debug.Log ("Save called");
		//Get Asset for saving
		containerCache = asset;

		if (containerCache == null)
		{
			UnityEditor.EditorUtility.DisplayDialog ("Invalid asset!", "Please contact Lara!", "OK");
			return;
		}

		//No edges no data for saving
		if (!Edges.Any()) return;

		var dialogContainer = ScriptableObject.CreateInstance <DialogContainer>();
		var connectedPorts = Edges.Where (x => x.input.node != null).ToArray();

		//Create Link list
		for (int i = 0; i < connectedPorts.Length; i++)
		{
			var outputNode = connectedPorts[i].output.node as DialogNode;
			var inputNode = connectedPorts[i].input.node as DialogNode;

			dialogContainer.nodeLinks.Add (new NodeLinkData
			{
				baseNodeGuid = outputNode.data.Guid,
				portName = connectedPorts[i].output.portName,
				targetNodeGuid = inputNode.data.Guid
			});
		}

		//Create Node List
		foreach (var dialogNode in Nodes.Where (node => !node.entryPoint))
		{
			DialogNodeData newData = dialogNode.data;
			newData.Position = dialogNode.GetPosition().position;
			dialogContainer.dialogNodeData.Add (newData);
		}

		foreach (Group i in Groups())
		{
			DialogGroupData tmpGroup = new DialogGroupData
			{
				name = i.title,
				Position = i.GetPosition().position
			};

			foreach (var node in i.containedElements)
			{
				if (node is DialogNode)
					tmpGroup.childNodeGuid.Add (((DialogNode)node).data.Guid);
			}

			dialogContainer.dialogGroupData.Add (tmpGroup);
		}

		foreach (string i in targetGraphView.blackBoardProperties)
			dialogContainer.exposedProperties.Add (i);

		//Update Scriptable Object
		containerCache.dialogNodeData = dialogContainer.dialogNodeData;
		containerCache.nodeLinks = dialogContainer.nodeLinks;
		containerCache.dialogGroupData = dialogContainer.dialogGroupData;
		containerCache.exposedProperties = dialogContainer.exposedProperties;
		UnityEditor.EditorUtility.SetDirty (containerCache);
		UnityEditor.AssetDatabase.SaveAssets();
	}

	public void LoadGraph (DialogContainer asset)
	{		
		//Get Asset for loading
		containerCache = asset;

		if (containerCache == null)
		{
			UnityEditor.EditorUtility.DisplayDialog ("Invalid asset!", "Please contact Lara!", "OK");
			return;
		}

		ClearGraph();

		CreateBlackBoard();
		CreateNodes();
		ConnectNodes();
		CreateGroups();
	}

	private void CreateBlackBoard()
	{
		foreach (string i in containerCache.exposedProperties)
			targetGraphView.AddPropertyToBlackBoard (i);
	}

	private void CreateGroups()
	{
		foreach (var group in containerCache.dialogGroupData)
		{
			var tmpGroup = DialogGraphUtility.CreateGroup (group.name, group.Position);

			for (int i = 0; i < Nodes.Count; ++i)
			{
				if (group.childNodeGuid.Contains (Nodes[i].data.Guid))
					tmpGroup.AddElement (Nodes[i]);
			}

			targetGraphView.AddElement (tmpGroup);
			//targetGraphView.AddElement (tmpNode);

			//var nodePorts = containerCache.nodeLinks.Where(x=> x.baseNodeGuid == nodeData.Guid).ToList();
			//nodePorts.ForEach (x=>tmpNode.CreateChoicePort (x.portName));
		}
	}

	private void ConnectNodes()
	{
		for (int i = 0; i < Nodes.Count; ++i)
		{
			DialogNode tmpNode = Nodes[i];
			var connections = containerCache.nodeLinks.Where(x=>x.baseNodeGuid == tmpNode.data.Guid).ToList();

			for (int j = 0; j < connections.Count; ++j)
			{
				var targetNodeGuid = connections[j].targetNodeGuid;
				var targetNode = Nodes.First (x=>x.data.Guid == targetNodeGuid);
				LinkNodes (tmpNode.outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
			}
		}
	}

	private void LinkNodes(Port output, Port input)
	{
		var tmpEdge = new Edge
		{
			output = output,
			input = input
		};

		tmpEdge.input.Connect (tmpEdge);
		tmpEdge.output.Connect (tmpEdge);
		targetGraphView.Add (tmpEdge);
	}

	private void CreateNodes()
	{
		foreach (var nodeData in containerCache.dialogNodeData)
		{
			DialogNode tmpNode;

			//Create a copy of data to prevent accidental override
			DialogNodeData data = nodeData.Copy();

			if (data.Type == DialogNodeData.NodeType.RAISE_EVENT)
				tmpNode = DialogRaiseEventNode.Create (data);
			else if (data.Type == DialogNodeData.NodeType.LISTEN_EVENT)
				tmpNode = DialogListenEventNode.Create (data);
			else if (data.Type == DialogNodeData.NodeType.NPC)
				tmpNode = DialogNpcNode.Create (data);
			else if (data.Type == DialogNodeData.NodeType.LOGIC)
				tmpNode = DialogLogicNode.Create (data, targetGraphView);
			else
				tmpNode = DialogChoiceNode.Create (data, targetGraphView);
			
			targetGraphView.AddElement (tmpNode);
			tmpNode.SetPosition (new Rect (data.Position, DialogNode.defaultSize));

			if (data.Type == DialogNodeData.NodeType.CHOICE)
			{
				DialogChoiceNode node = (DialogChoiceNode) tmpNode;
				var nodePorts = containerCache.nodeLinks.Where(x=> x.baseNodeGuid == data.Guid).ToList();
				nodePorts.ForEach (x=>node.CreateChoicePort (x.portName));
			}
		}
	}

	private void ClearGraph()
	{
		//Setzt the root guid if available
		if (containerCache.nodeLinks.Count > 0)
			Nodes.Find (x => x.entryPoint).data.Guid = containerCache.nodeLinks[0].baseNodeGuid;

		foreach (var node in Nodes)
		{
			if (node.entryPoint) continue;

			//Remove Edges
			Edges.Where (x => x.input.node ==node).ToList().ForEach(edge=>targetGraphView.RemoveElement(edge));

			targetGraphView.RemoveElement (node);
		}

		foreach (Group group in Groups())
			targetGraphView.RemoveElement (group);

		targetGraphView.ClearBlackBoard();
	}

	private List <Group> Groups ()
	{
		VisualElement root = targetGraphView.contentViewContainer;
		List<Group> groupList = new List<Group>();
		root.Query<Group>().ForEach(group => groupList.Add(group));
		return groupList;
	}
}

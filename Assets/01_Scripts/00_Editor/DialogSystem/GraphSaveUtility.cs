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
				baseNodeGuid = outputNode.GUID,
				portName = connectedPorts[i].output.portName,
				targetNodeGuid = inputNode.GUID
			});
		}

		//Create Node List
		foreach (var dialogNode in Nodes.Where (node => !node.entryPoint))
		{
			dialogContainer.dialogNodeData.Add (new DialogNodeData
			{
				Guid = dialogNode.GUID,
				DialogText = dialogNode.dialogText,
				Position = dialogNode.GetPosition().position,
				DialogTitle = dialogNode.title
				
			});
		}

		//Update Scriptable Object
		containerCache.dialogNodeData = dialogContainer.dialogNodeData;
		containerCache.nodeLinks = dialogContainer.nodeLinks;
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
		CreateNodes();
		ConnectNodes();
	}

	private void ConnectNodes()
	{
		for (int i = 0; i < Nodes.Count; ++i)
		{
			var k = i; //Prevent access to modified closure
			var connections = containerCache.nodeLinks.Where(x=>x.baseNodeGuid == Nodes[k].GUID).ToList();

			for (int j = 0; j < connections.Count; ++j)
			{
				var targetNodeGuid = connections[j].targetNodeGuid;
				var targetNode = Nodes.First (x=>x.GUID == targetNodeGuid);
				LinkNodes (Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

				targetNode.SetPosition (new Rect (containerCache.dialogNodeData.First (x=>x.Guid == targetNodeGuid).Position,
					DialogNode.defaultSize));
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
			var tmpNode = targetGraphView.CreateDialogNode (nodeData);
			targetGraphView.AddElement (tmpNode);

			var nodePorts = containerCache.nodeLinks.Where(x=> x.baseNodeGuid == nodeData.Guid).ToList();
			nodePorts.ForEach (x=>tmpNode.CreateChoicePort (x.portName));
		}
	}

	private void ClearGraph()
	{
		//Setzt the root guid if available
		if (containerCache.nodeLinks.Count > 0)
			Nodes.Find (x => x.entryPoint).GUID = containerCache.nodeLinks[0].baseNodeGuid;

		foreach (var node in Nodes)
		{
			if (node.entryPoint) continue;

			//Remove Edges
			Edges.Where (x => x.input.node ==node).ToList().ForEach(edge=>targetGraphView.RemoveElement(edge));

			targetGraphView.RemoveElement (node);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]

public class RoomNodeGraphSO : ScriptableObject
{
	[HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
	[HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
	[HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

	private void Awake()
	{
		LoadRoomNodeDictionary();
	}

	// load the room node dictionary from the room node list
	private void LoadRoomNodeDictionary()
	{
		roomNodeDictionary.Clear();

		// populate dictionary
		foreach (RoomNodeSO node in roomNodeList)
		{
			roomNodeDictionary[node.id] = node;
		}
	}

	// get room node by roomNodeType
	public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
	{
		foreach (RoomNodeSO node in roomNodeList)
		{
			if (node.roomNodeType == roomNodeType)
			{
				return node;
			}
		}

		return null;
	}

	// get room node by room nodeID
	public RoomNodeSO GetRoomNode(string roomNodeID)
	{
		if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
		{
			return roomNode;
		}

		return null;
	}

	// get child room nodes for supplied parent room node
	public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
	{
		foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
		{
			yield return GetRoomNode(childNodeID);
		}
	}

	#region Editor Code

#if UNITY_EDITOR
	[HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
	[HideInInspector] public Vector2 linePosition;

	// repopulate node dictionary every time a change is made in tthe editor
	public void OnValidate() {
		LoadRoomNodeDictionary();
	}

	public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
	{
		roomNodeToDrawLineFrom = node;
		linePosition = position;
	}

#endif

	#endregion
}

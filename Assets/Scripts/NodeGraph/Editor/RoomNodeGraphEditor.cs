using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
	private GUIStyle roomNodeStyle;
	private GUIStyle roomNodeSelectedStyle;
	private static RoomNodeGraphSO currentRoomNodeGraph;
	private RoomNodeSO currentRoomNode = null;
	private RoomNodeTypeListSO roomNodeTypeList;
	private Vector2 graphOffset;
	private Vector2 graphDrag;

	// node layout values
	private const float nodeWidth = 160f;
	private const float nodeHeigth = 75f;
	private const int nodePadding = 25;
	private const int nodeBorder = 12;

	// connecting line values
	private const float connectingLineWidth = 3f;
	private const float connectingLineArrowSize = 6f;

	// grid spacing
	private const float gridLarge = 100f;
	private const float gridSmall = 25f;

	[MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

	private static void OpenWindow()
	{
		GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
	}

	private void OnEnable()
	{
		// subscribe to the inspector selection changed event
		Selection.selectionChanged += InspectorSelectionChanged;

		// define node layout style
		roomNodeStyle = new GUIStyle();
		roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
		roomNodeStyle.normal.textColor = Color.white;
		roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		// define selected node style
		roomNodeSelectedStyle = new GUIStyle();
		roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
		roomNodeSelectedStyle.normal.textColor = Color.white;
		roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		// load room node types
		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
	}

	private void OnDisable()
	{
		// unsubscribe from the inspector selection changed event
		Selection.selectionChanged -= InspectorSelectionChanged;
	}

	// open the room node graph editor window if a room node graph scriptable asset is double clicked in the inspector
	[OnOpenAsset(0)] // need the namespace UnityEditor.Callbacks

	public static bool OnDoubleClickAsset(int instanceID, int line)
	{
		RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

		if (roomNodeGraph)
		{
			OpenWindow();
			currentRoomNodeGraph = roomNodeGraph;

			return true;
		}

		return false;
	}

	// draw editoe gui
	private void OnGUI()
	{
		// if a scriptable object of type RoomNodeGraphSO has been selected then process
		if (currentRoomNodeGraph)
		{
			// draw grid
			DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
			DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

			// draw line if being dragged
			DrawDraggedLine();

			// process events
			ProcessEvents(Event.current);

			// draw connections between room nodes
			DrawRoomConnections();

			// draw room nodes
			DrawRoomNodes();
		}

		if (GUI.changed)
		{
			Repaint();
		}
	}

	// draw a background grid for the room node graph editor
	private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
	{
		int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
		int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);
		Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

		Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
		graphOffset += graphDrag * 0.5f;

		for (int i = 0; i < verticalLineCount; i++)
		{
			Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
		}

		for (int j = 0; j < horizontalLineCount; j++)
		{
			Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
		}

		Handles.color = Color.white;
	}

	private void DrawDraggedLine()
	{
		if (currentRoomNodeGraph.linePosition != Vector2.zero)
		{
			// draw line from node to line position
			Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
				currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
		}
	}

	private void ProcessEvents(Event currentEvent)
	{
		// reset graph drag
		graphDrag = Vector2.zero;

		// get room node that mouse is over if it is null or not currently being dragged
		if (!currentRoomNode || !currentRoomNode.isLeftClickDragging)
		{
			currentRoomNode = IsMouseOverRoomNode(currentEvent);
		}

		// if mouse is not over a room node or we are currently dragging a line from the room node then process graph events
		if (!currentRoomNode || currentRoomNodeGraph.roomNodeToDrawLineFrom)
		{
			ProcessRoomNodeGraphEvents(currentEvent);
		}

		// else process room node events
		else
		{
			currentRoomNode.ProcessEvents(currentEvent);
		}
	}

	// check to see mouse is over a room node - if so then return the room node else return null
	private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
	{
		for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
		{
			if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
			{
				return currentRoomNodeGraph.roomNodeList[i];
			}
		}

		return null;
	}

	// process room node graph events
	private void ProcessRoomNodeGraphEvents(Event currentEvent)
	{
		switch (currentEvent.type)
		{
			// process mouse down events
			case EventType.MouseDown:
				ProcessMouseDownEvent(currentEvent);
				break;

			// process mouse up events
			case EventType.MouseUp:
				ProcessMouseUpEvent(currentEvent);
				break;

			// process mouse drag event
			case EventType.MouseDrag:
				ProcessMouseDragEvent(currentEvent);
				break;

			default:
				break;
		}
	}

	// process mouse down events on the room node graph (not over a node)
	private void ProcessMouseDownEvent(Event currentEvent)
	{
		// process right click mouse down on graph event (show context menu
		if (currentEvent.button == 1)
		{
			ShowContextMenu(currentEvent.mousePosition);
		}

		// process left mouse down on graph event
		else if (currentEvent.button == 0)
		{
			ClearLineDrag();
			ClearAllSelectedRoomNodes();
		}
	}

	// show the context menu
	private void ShowContextMenu(Vector2 mousePosition)
	{
		GenericMenu menu = new GenericMenu();
		menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
		menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);
		menu.ShowAsContext();
	}

	// create a room node at the mouse position
	private void CreateRoomNode(object mousePositionObject)
	{
		// if current node graph empty then add entrance room node first
		if (currentRoomNodeGraph.roomNodeList.Count == 0)
		{
			CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
		}

		CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
	}

	// create a room node at the mouse position - overloaded to also pass in RoomNodeType
	private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
	{
		Vector2 mousePosition = (Vector2)mousePositionObject;

		// create room node scriptable object asset
		RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

		// add room node to current room node graph room node list
		currentRoomNodeGraph.roomNodeList.Add(roomNode);

		// set room node values
		roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeigth)), currentRoomNodeGraph, roomNodeType);

		// add room node to room node graph scriptable object asset database
		AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
		AssetDatabase.SaveAssets();

		// refresh graph node dictionary
		currentRoomNodeGraph.OnValidate();
	}

	// delete the links between the selected room nodes
	private void DeleteSelectedRoomNodeLinks()
	{
		// iterate through all room nodes
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
			{
				for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
				{
					// get child room node
					RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

					// if the chiild room node is selected
					if (childRoomNode && childRoomNode.isSelected)
					{
						// remove childID from parent room node
						roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

						// remove parentID from child room node
						childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
					}
				}
			}
		}
	}

	// delete selected room nodes
	private void DeleteSelectedRoomNodes()
	{
		Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

		// loop through all nodes
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
			{
				roomNodeDeletionQueue.Enqueue(roomNode);

				// iterate through child room node ids
				foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
				{
					// retrieve child room node
					RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

					if (childRoomNode)
					{
						// remove parentID from child room node
						childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
					}
				}

				// iterate through parent room node ids
				foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
				{
					// retrieve parent node
					RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

					if (parentRoomNode)
					{
						// remove childID from parent node
						parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
					}
				}
			}
		}

		// delete queued room nodes
		while (roomNodeDeletionQueue.Count > 0)
		{
			// get room node from queue
			RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

			// remove node from dictionary
			currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

			// remove node from list
			currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

			// remove node from asset database
			DestroyImmediate(roomNodeToDelete, true);

			// save asset database
			AssetDatabase.SaveAssets();
		}
	}

	// clear selection from all room nodes
	private void ClearAllSelectedRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected)
			{
				roomNode.isSelected = false;
				GUI.changed = true;
			}
		}
	}

	// select all room nodes
	private void SelectAllRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			roomNode.isSelected = true;
		}

		GUI.changed = true;
	}

	// process mouse up events
	private void ProcessMouseUpEvent(Event currentEvent)
	{
		// if releasing the right mouse button and currently dragging a line
		if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom)
		{
			// check if over a room node
			RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

			if (roomNode)
			{
				// if so set it as a child of the parent room node if it can be added
				if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
				{
					// set parent id in child room node
					roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
				}
			}

			ClearLineDrag();
		}
	}

	// process mouse drag event
	private void ProcessMouseDragEvent(Event currentEvent)
	{
		// process rigth click drag event - draw line
		if (currentEvent.button == 1)
		{
			ProcessRightMouseDragEvent(currentEvent);
		}

		// process left click drag event - drag node graph
		else if (currentEvent.button == 0)
		{
			ProcessLeftMouseDragEvent(currentEvent.delta);
		}
	}

	// process right mouse drag event - draw line
	private void ProcessRightMouseDragEvent(Event currentEvent)
	{
		if (currentRoomNodeGraph.roomNodeToDrawLineFrom)
		{
			DragConnectingLine(currentEvent.delta);
			GUI.changed = true;
		}
	}

	// process left mouse drag event - drag room node graph
	private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
	{
		graphDrag = dragDelta;

		for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
		{
			currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
		}

		GUI.changed = true;
	}

	// draw connecting line from room node
	public void DragConnectingLine(Vector2 delta)
	{
		currentRoomNodeGraph.linePosition += delta;
	}

	// clear line drag from a room node
	private void ClearLineDrag()
	{
		currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
		currentRoomNodeGraph.linePosition = Vector2.zero;
		GUI.changed = true;
	}

	// draw connections in the graph window between room nodes
	private void DrawRoomConnections()
	{
		// loop through all room nodes
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.childRoomNodeIDList.Count > 0)
			{
				// loop through child room nodes
				foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
				{
					// get child room node from dictionary
					if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
					{
						DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
						GUI.changed = true;
					}
				}
			}
		}
	}

	// draw connection line between the parent room node and child room node
	private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
	{
		// get line start and end position
		Vector2 startPosition = parentRoomNode.rect.center;
		Vector2 endPosition = childRoomNode.rect.center;

		// calculate midway point
		Vector2 midPosition = (endPosition + startPosition) / 2f;

		// vector from start to end position of line
		Vector2 direction = endPosition - startPosition;

		// calculate normalised perpendicular positions from the mid point
		Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
		Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

		// calculate mid point offset position for arrow head
		Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

		// draw arrow
		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

		// draw line
		Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

		GUI.changed = true;

	}

	// draw room nodes in the graph window
	private void DrawRoomNodes()
	{
		// loop through all room nodes and draw them
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected)
			{
				roomNode.Draw(roomNodeSelectedStyle);
			}

			else
			{
				roomNode.Draw(roomNodeStyle);
			}
		}

		GUI.changed = true;
	}

	// selection changed in the inspector
	private void InspectorSelectionChanged()
	{
		RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

		if (roomNodeGraph)
		{
			currentRoomNodeGraph = roomNodeGraph;
			GUI.changed = true;
		}
	}
}
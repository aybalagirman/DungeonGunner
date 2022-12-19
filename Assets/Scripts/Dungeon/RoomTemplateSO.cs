using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]

public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

	#region
	[Space(10)]
	[Header("ROOM PREFAB")]
	#endregion

	#region Tooltip
	[Tooltip("The game object prefab for the room (his will contain all the tilemaps for the room and environment game objects")]
	#endregion
	public GameObject prefab;
	[HideInInspector] public GameObject previousPrefab; // this is used to regenerate he guid if the scriptable object is copied nd the prefab is changed

	#region Header ROOM CONFIGURATION
	[Space(10)]
	[Header("ROOM CONFIGURATION")]

	#endregion Header ROOM CONFIGURATION

	#region Tooltip
	[Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph. The exception being with corridors. In the room node graph there is just one corridor type 'Corridor'. For the room templates there are two corridor node types - Corridor NS and CorridorEW")]
	#endregion Tooltip
	public RoomNodeTypeSO roomNodeType;

	#region Tooltip
	[Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner) - note: this is the local tilemap position and not the world position")]
	#endregion Tooltip
	public Vector2Int lowerBounds;

	#region Tooltip
	[Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner) - note: this is the local tilemap position and not the world position")]
	#endregion Tooltip
	public Vector2Int upperBounds;

	#region Tooltip
	[Tooltip("There should be a maximum of four doorways for a room - one for each compass direction. These should have a consistent three tile opening size, with the middle tile position being the doorway coordinate 'position'")]
	#endregion Tooltip
	[SerializeField] public List<Doorway> doorwayList;

	#region Tooltip
	[Tooltip("Each possible spawn position (used for enemies and chest) for the room in tilemap coordinates should be added to this array")]
	#endregion Tooltip
	public Vector2Int[] spawnPositionArray;

	// returns a list of entrances for the room template
	public List<Doorway> GetDoorwayList()
	{
		return doorwayList;
	}

	#region Validation
#if UNITY_EDITOR
	// validate SO fields
	private void OnValidate()
	{
		// set unique GUID if empty or the prefab changes
		if (guid == "" || previousPrefab != prefab)
		{
			guid = GUID.Generate().ToString();
			previousPrefab = prefab;
			EditorUtility.SetDirty(this);
		}

		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

		// check spawn positions populated
		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
	}
#endif

	#endregion Validation
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
	#region DUNGEON BUILD SETTINGS
	public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
	public const int maxDungeonBuildAttempts = 10;
	#endregion

	#region ROOM SETTINGS
	public const int maxChildCorridors = 3; // max number of child corridors leading from a room. maximum should be three since it can cause the dungeon building to fail since the rooms are more likely not to fit together.
	#endregion
}

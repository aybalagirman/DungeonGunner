using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
	[HideInInspector] public Room room;
	[HideInInspector] public Grid grid;
	[HideInInspector] public Tilemap groundTilemap;
	[HideInInspector] public Tilemap decoration1Tilemap;
	[HideInInspector] public Tilemap decoration2Tilemap;
	[HideInInspector] public Tilemap frontTilemap;
	[HideInInspector] public Tilemap collisionTilemap;
	[HideInInspector] public Tilemap minimapTilemap;
	[HideInInspector] public Bounds roomColliderBounds;
	private BoxCollider2D boxCollider2D;

	private void Awake() {
		boxCollider2D = GetComponent<BoxCollider2D>();

		// save room collider bounds
		roomColliderBounds = boxCollider2D.bounds;
	}

	// initialise the instantiated room
	public void Initialise(GameObject roomGameObject)
	{
		PopulateTilemapMemberVariables(roomGameObject);
		BlockOffUnusedDoorways();
		DisableCollisionTilemapRenderer();
	}

	// populate the tilemap and grid member variables
	private void PopulateTilemapMemberVariables(GameObject roomGameObject)
	{
		// get the grid component
		grid = roomGameObject.GetComponentInChildren<Grid>();

		// get tilemaps in children
		Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

		foreach (Tilemap tilemap in tilemaps)
		{
			if (tilemap.gameObject.tag == "groundTilemap")
			{
				groundTilemap = tilemap;
			}

			else if (tilemap.gameObject.tag == "decoretion1Tilemap")
			{
				decoration1Tilemap = tilemap;
			}

			else if (tilemap.gameObject.tag == "decoretion2Tilemap") {
				decoration2Tilemap = tilemap;
			}

			else if (tilemap.gameObject.tag == "frontTilemap") {
				frontTilemap = tilemap;
			}

			else if (tilemap.gameObject.tag == "collisionTilemap") {
				collisionTilemap = tilemap;
			}

			else if (tilemap.gameObject.tag == "minimapTilemap") {
				minimapTilemap = tilemap;
			}
		}
	}

	// block off unused doorways in a room
	private void BlockOffUnusedDoorways()
	{
		// loop through all doorways
		foreach (Doorway doorway in room.doorwayList)
		{
			if (doorway.isConnected)
			{
				continue;
			}

			// block unconnected doorways using tiles on tilemaps
			if (collisionTilemap)
			{
				BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
			}

			if (minimapTilemap)
			{
				BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
			}

			if (groundTilemap)
			{
				BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
			}

			if (decoration1Tilemap)
			{
				BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
			}

			if (decoration2Tilemap)
			{
				BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
			}

			if (frontTilemap)
			{
				BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
			}
		}
	}

	// block a doorway on a tilemap layer
	private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
	{
		switch (doorway.orientation)
		{
			case Orientation.north:
			case Orientation.south:
				BlockDoorwayHorizontally(tilemap, doorway);
				break;

			case Orientation.east:
			case Orientation.west:
				BlockDoorwayVertically(tilemap, doorway);
				break;

			case Orientation.none:
				break;
		}
	}

	// block doorways horizontally - for north and south doorways
	private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
	{
		Vector2Int startPosition = doorway.doorwayStartCopyPosition;

		// loop through all tiles to copy
		for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
		{
			for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
			{
				// get rotation of tile being copied
				Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

				// copy tile
				tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

				// set rotation of tile copied
				tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
			}
		}
	}

	// block doorways vertically - for east and west doorways
	private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
	{
		Vector2Int startPosition = doorway.doorwayStartCopyPosition;

		// loop through all tiles to copy
		for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
		{
			for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
			{
				// get rotation of tile being copied
				Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

				// copy tile
				tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

				// set rotation of tile copied
				tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
			}
		}
	}

	// disable collision tilemap renderer
	private void DisableCollisionTilemapRenderer()
	{
		collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
	}
}

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

	// disable collision tilemap renderer
	private void DisableCollisionTilemapRenderer()
	{
		collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
	}
}

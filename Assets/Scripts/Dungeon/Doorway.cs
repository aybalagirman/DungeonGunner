using UnityEngine;
[System.Serializable]

public class Doorway
{
    public Vector2Int position;
    public Orientation orientation;
    public GameObject doorPrefab;

    #region Header
    [Header("The upper left position to start copying from")]
    #endregion
    public Vector2Int doorwayStartCopyPosition;
    #region Header
    [Header("The width of tiles in the doorway to copy over")]
    #endregion
    public int doorwayCopyTileWidth;
    #region Header
    [Header("The height of the tiles n the doorway to copy over")]
    #endregion
    public int doorwayCopyTileHeight;
    [HideInInspector]
    public bool isConnected = false;
    [HideInInspector]
    public bool isUnavailable = false;

}

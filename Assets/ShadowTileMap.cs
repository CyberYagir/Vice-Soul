using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowTileMap : MonoBehaviour
{
    public Transform localPlayer;
    public Tilemap tilemap, main;
    public Vector2Int size;
    public int playerLight;
    PlayerStats player;
    public TileBase shadowBlock, shadowBlock1;
    private void Start()
    {
        player = localPlayer.GetComponent<PlayerStats>();
    }
    private void Update()
    {
        playerLight = player.localPlayer.lightRange;
        Vector3Int ppos = tilemap.WorldToCell(localPlayer.position);
        for (int x = -size.x; x < size.x; x++)
        {
            for (int y = -size.y; y < size.y; y++)
            {
                var newpos = ppos + new Vector3Int(x, y, 0);

                if (main.GetTile(newpos))
                {
                    tilemap.SetTile(newpos, shadowBlock);

                    if (main.GetTile(ppos + new Vector3Int(x, y + 2, 0)) == null ||
                           main.GetTile(ppos + new Vector3Int(x, y - 2, 0)) == null ||
                           main.GetTile(ppos + new Vector3Int(x + 2, y, 0)) == null ||
                           main.GetTile(ppos + new Vector3Int(x - 2, y, 0)) == null ||
                           main.GetTile(ppos + new Vector3Int(x + 2, y - 2, 0)) == null ||
                           main.GetTile(ppos + new Vector3Int(x - 2, y - 2, 0)) == null ||
                           main.GetTile(ppos + new Vector3Int(x - 2, y + 2, 0)) == null ||
                           main.GetTile(ppos + new Vector3Int(x + 2, y + 2, 0)) == null)
                    {
                        tilemap.SetTile(ppos + new Vector3Int(x, y, 0), shadowBlock1);
                    }
                }
                else
                {
                    tilemap.SetTile(newpos, null);
                }
                if (main.GetTile(newpos) != null)
                {
                    if (main.GetTile(ppos + new Vector3Int(x, y + 1, 0)) == null ||
                        main.GetTile(ppos + new Vector3Int(x, y - 1, 0)) == null ||
                        main.GetTile(ppos + new Vector3Int(x + 1, y, 0)) == null ||
                        main.GetTile(ppos + new Vector3Int(x - 1, y, 0)) == null ||
                        main.GetTile(ppos + new Vector3Int(x + 1, y - 1, 0)) == null ||
                        main.GetTile(ppos + new Vector3Int(x - 1, y - 1, 0)) == null ||
                        main.GetTile(ppos + new Vector3Int(x - 1, y + 1, 0)) == null ||
                        main.GetTile(ppos + new Vector3Int(x + 1, y + 1, 0)) == null)
                    {
                        tilemap.SetTile(ppos + new Vector3Int(x, y, 0), null);
                    }
                    
                }
            }
        }
        for (int x = -playerLight; x < playerLight+1; x++)
        {
            for (int y = -playerLight; y < playerLight+1; y++)
            {
                tilemap.SetTile(ppos + new Vector3Int(x, y, 0), null);
            }
        }
    }
}

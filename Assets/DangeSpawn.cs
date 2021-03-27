using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DangeSpawn : MonoBehaviour
{
    public Tilemap thisMain, thisBack;
    public Tilemap worldMain, worldBack;
    public Vector2Int size;
    public TileBase setEmpty;
    public GameObject[] entities;
    public void Spawn()
    {
        worldMain = FindObjectOfType<WorldGenerator>().main;
        worldBack = FindObjectOfType<WorldGenerator>().back;

        for (int y = size.y; y < 0; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (thisBack.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    worldBack.SetTile(Vector3Int.CeilToInt(transform.position + new Vector3(x, y)), thisBack.GetTile(new Vector3Int(x, y, 0)));
                }
                if (thisMain.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    if (thisMain.GetTile(new Vector3Int(x, y, 0)) == setEmpty)
                    {
                        worldMain.SetTile(Vector3Int.CeilToInt(transform.position + new Vector3(x, y)), null);
                        continue;
                    }
                    worldMain.SetTile(Vector3Int.CeilToInt(transform.position + new Vector3(x, y)), thisMain.GetTile(new Vector3Int(x, y, 0)));

                }
            }
        }
        for (int i = 0; i < entities.Length; i++)
        {
            entities[i].transform.parent = null;
            entities[i].SetActive(true);
        }
        Destroy(gameObject);
    }

}

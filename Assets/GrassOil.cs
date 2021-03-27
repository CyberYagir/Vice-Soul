using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassOil : MonoBehaviour
{
    public int boomRadius;
    public ParticleSystem particles;

    private void Start()
    {
        Boom();
    }

    public void Boom()
    {

        Tilemap main = FindObjectOfType<WorldGenerator>().main;
        Tilemap decor = FindObjectOfType<WorldGenerator>().decor;

        var pos = main.WorldToCell(transform.position + new Vector3(0.5f, 0.5f));
        pos = new Vector3Int(pos.x, pos.y, 0);

        for (int i = -boomRadius; i < boomRadius; i++)
        {
            for (int j = -boomRadius; j < boomRadius; j++)
            {
                if (Vector3.Distance((Vector3)pos, (Vector3)pos + new Vector3(i, j)) < boomRadius)
                {
                    var tileBases = FindObjectOfType<WorldGenerator>().tileBases;
                    var tile = main.GetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j)));

                    
                    if (tile == tileBases[2] || tile == tileBases[16] || tile == tileBases[17] || tile == tileBases[0])
                    {
                        if (tile == tileBases[2])
                        {
                            decor.SetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j + 1)), tileBases[Random.Range(10, 12)]);
                        }

                        if (main.GetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j + 1))) == null)
                        {
                            main.SetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j)), tileBases[2]);
                            if (tile != tileBases[2])
                            {
                                decor.SetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j + 1)), null);
                            }
                        }
                        else
                        {
                            main.SetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j)), tileBases[0]);
                        }
                    }
                    
                }
            }
        }

        particles.transform.parent = null;
        particles.Play();
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TNT : MonoBehaviour
{
    public int boomRadius;
    public ParticleSystem particles;
    public EnergyActive energyActive;


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
                    main.SetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j)), null);
                    decor.SetTile(Vector3Int.CeilToInt((Vector3)pos + new Vector3(i, j)), null);
                }
            }
        }
        var allObj = FindObjectsOfType<Entity>();
        for (int i = 0; i < allObj.Length; i++)
        {
            if (Vector2.Distance((Vector2)allObj[i].transform.position, new Vector2(pos.x, pos.y)) < boomRadius + 2)
            {
                if (allObj[i].GetComponent<Tree>() != null)
                {
                    allObj[i].GetComponent<Tree>().hp = 0;
                    continue;
                }
                Destroy(allObj[i].gameObject);
            }
        }
        var allbm = FindObjectsOfType<MobStats>();
        for (int i = 0; i < allbm.Length; i++)
        {
            var dst = Vector2.Distance((Vector2)allbm[i].transform.position, new Vector2(pos.x, pos.y));
            if (dst < boomRadius + 2)
            {
                allbm[i].hp -= 200 / dst;
            }
        }

        Player2D pl = FindObjectOfType<Player2D>();
        var pldist = Vector2.Distance((Vector2)pl.transform.position, new Vector2(pos.x, pos.y));
        if (pldist < boomRadius + 2)
        {
            pl.GetComponent<PlayerStats>().localPlayer.health -= (int)(50 / pldist);

        }

        particles.transform.parent = null;
        particles.Play();
        Destroy(gameObject);
    }

    private void Update()
    {
        if (energyActive != null)
        {
            if (energyActive.active)
            {
                Boom();
            }
        }
    }

}

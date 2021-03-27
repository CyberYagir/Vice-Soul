using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fluid : MonoBehaviour
{
    public LayerMask mask;
    public LayerMask layerFluid;

    Vector2 hitPoint;

    public bool end;

    Vector2 pos;
    float dst = -99999;

    public float dist;
    public Water2D.Water2D_Spawner water;
    Transform player;

    public bool skip;

    private void Start()
    {
        player = FindObjectOfType<Player2D>().transform;
        if (skip == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 50, mask);
            if (hit.collider != null)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red, 5);
                transform.position = hit.point;
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }

            hit = Physics2D.Raycast(transform.position + new Vector3(1, 0), Vector2.right, 50, mask);
            if (hit.collider == null)
            {
                gameObject.SetActive(false);
                return;
            }
            Debug.DrawLine(transform.position, hit.point, Color.yellow, 20);


            for (int i = (int)transform.position.x; i < (int)hit.point.x; i++)
            {
                RaycastHit2D hit2 = Physics2D.Raycast(new Vector3(i + 0.5f, transform.position.y, transform.position.z), Vector2.down, 50, mask);
                Debug.DrawLine(new Vector3(i + 0.5f, transform.position.y, transform.position.z), hit2.point, Color.blue, 20);
                if (Vector2.Distance(hit2.point, new Vector3(i + 0.5f, transform.position.y, transform.position.z)) > dst)
                {
                    dst = Vector2.Distance(hit2.point, new Vector3(i + 0.5f, transform.position.y, transform.position.z));
                    pos = hit2.point;
                }
            }
            if (transform.position == new Vector3(0, 0.5f, 0))
            {
                gameObject.SetActive(false);
                return;
            }

            water.transform.position = pos + new Vector2(0, 0.5f);

            RaycastHit2D endhit = Physics2D.Raycast(transform.position, Vector2.left, 50, mask);
            Debug.DrawRay(transform.position, Vector2.left, Color.red, 10);
            transform.position = endhit.point;
            if (endhit.collider != null)
            {
                transform.position = endhit.point;
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }
            water.transform.position = endhit.point + new Vector2(0.01f, 0);
            endhit = Physics2D.Raycast(transform.position + new Vector3(0.1f, 0), Vector2.right, 50, mask);
            if (endhit.collider == null)
            {
                gameObject.SetActive(false);
                return;
            }
            Debug.DrawLine(transform.position, endhit.point, Color.yellow, 20);

            GameObject @object = water.WaterDropsObjects[0];
            var g = (int)((System.Math.Ceiling(endhit.point.x) - transform.position.x) + 1) * 10;
            if (g > 300)
            {
                g = 300;
            }
            water.WaterDropsObjects = new GameObject[g];
            water.WaterDropsObjects[0] = @object;
            water.enabled = true;

        }
        else
        {
            StartCoroutine(waitSkip());
        }
    }

    private void Update()
    {
        if (end == false)
        {
            bool all = true;
            for (int i = 0; i < water.WaterDropsObjects.Length; i++)
            {
                if (water.WaterDropsObjects[i] == null)
                {
                    all = false;
                    return;
                }
                if (water.WaterDropsObjects[i].active == true)
                {
                    water.WaterDropsObjects[i].active = false;
                }
            }
            


            if (all)
            {
                water.enabled = false;
                water.StopAllCoroutines();
                for (int i = 0; i < water.WaterDropsObjects.Length; i++)
                {
                    water.WaterDropsObjects[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-4, 4), 0), ForceMode2D.Impulse);
                }
                StartCoroutine(lod());
                end = true;
            }
        }
        else
        {
            if (Vector2.Distance((Vector2)transform.position, (Vector2)player.position) <= dist)
            {
                water.WaterMaterial.SetColor("_Color", water.FillColor);
                water.WaterMaterial.SetColor("_StrokeColor", water.StrokeColor);
            }
        }
    }

    IEnumerator waitSkip()
    {
        yield return new WaitForSeconds(0.1f);
        RaycastHit2D endhit = Physics2D.Raycast(transform.position + new Vector3(0.1f, 0), Vector2.right, 50, mask);
        if (endhit.collider == null)
        {
            gameObject.SetActive(false);
            yield break;
        }
        Debug.DrawLine(transform.position, endhit.point, Color.yellow, 20);

        GameObject @object = water.WaterDropsObjects[0];
        var g = (int)((System.Math.Ceiling(endhit.point.x) - transform.position.x) + 1) * 10;
        if (g > 300)
        {
            g = 300;
        }
        water.WaterDropsObjects = new GameObject[g];
        water.WaterDropsObjects[0] = @object;
        water.enabled = true;
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < water.WaterDropsObjects.Length; i++)
        {
            water.WaterDropsObjects[i].SetActive(true);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0), Vector2.right, Mathf.Infinity, mask);
            Debug.DrawLine(transform.position, hit.point, Color.blue, 10);
            water.WaterDropsObjects[i].transform.position = hit.point - new Vector2((i + 1) * 0.1f, 0);
            water.WaterDropsObjects[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(4, 0), ForceMode2D.Impulse);
        }
    }

    IEnumerator lod()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < water.WaterDropsObjects.Length; i++)
            {
                yield return new WaitForSeconds(0.1f);
                if (Vector2.Distance((Vector2)water.WaterDropsObjects[i].transform.position, (Vector2)player.transform.position) <= dist)
                {
                    water.WaterDropsObjects[i].SetActive(true);
                    water.WaterDropsObjects[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1, 1), 0), ForceMode2D.Impulse);

                }
                else
                {
                    water.WaterDropsObjects[i].SetActive(false);
                }
            }
        }
        
    }

    #region MyRegion
    //    public GameObject player;
    //    public Tilemap map, map2, main;
    //    public TileBase tile, left, right;


    //    public float updateSpeed;

    //    public List<Vector3Int> tiles = new List<Vector3Int>();

    //    public int litles = 10;

    //    IEnumerator enumerator;

    //    public Vector3Int startPos;

    //    public bool started;


    //    private void Start()
    //    {
    //        enumerator = upd();
    //        StartCoroutine(enumerator);
    //        StartCoroutine(str());
    //        main = FindObjectOfType<WorldGenerator>().main;
    //    }

    //    private void Update()
    //    {
    //        if (enumerator == null)
    //        {
    //            enumerator = upd();
    //            StartCoroutine(enumerator);
    //        }
    //    }
    //    public IEnumerator str()
    //    {
    //        yield return new WaitForSeconds(2f);
    //        started = true;
    //    }
    //    public IEnumerator upd()
    //    {
    //        yield return new WaitForSeconds(2f);
    //        Vector3Int p = startPos;
    //        map.SetTile(p, tile);
    //        tiles.Add(p);
    //        while (true)
    //        {
    //            Vector3Int temp;
    //            for (int write = 0; write < tiles.Count; write++)
    //            {
    //                for (int sort = 0; sort < tiles.Count - 1; sort++)
    //                {
    //                    if (tiles[sort].y > tiles[sort + 1].y)
    //                    {
    //                        temp = tiles[sort + 1];
    //                        tiles[sort + 1] = tiles[sort];
    //                        tiles[sort] = temp;
    //                    }
    //                }
    //            }
    //            if (started == true)
    //            {
    //                yield return new WaitForSeconds(updateSpeed);
    //            }
    //            else
    //            {
    //                yield return new WaitForSeconds(0.05f);
    //            }
    //            map2.ClearAllTiles();
    //            for (int i = 0; i < tiles.Count; i++)
    //            {
    //                //Left
    //                if ((map.GetTile(tiles[i] + new Vector3Int(-1, 0, 0)) == null &&
    //                    main.GetTile(tiles[i] + new Vector3Int(-1, 0, 0)) == null &&
    //                    main.GetTile(tiles[i] + new Vector3Int(0, -1, 0)) != null &&
    //                    main.GetTile(tiles[i] + new Vector3Int(-1, -1, 0)) != null))
    //                {
    //                    map2.SetTile(tiles[i] + new Vector3Int(-1, 0, 0), right);
    //                }
    //                if (map.GetTile(tiles[i] + new Vector3Int(1, 1, 0)) != null)
    //                {
    //                    map2.SetTile(tiles[i] + new Vector3Int(0, 1, 0), right);
    //                }

    //                if (main.GetTile(tiles[i] + new Vector3Int(-1, -1, 0)) == null &&
    //                    map2.GetTile(tiles[i] + new Vector3Int(-1, 0, 0)) == null &&
    //                    map2.GetTile(tiles[i] + new Vector3Int(0, 1, 0)) == null &&
    //                    map.GetTile(tiles[i] + new Vector3Int(0, 1, 0)) == null)
    //                {
    //                    map.SetTile(tiles[i], right);
    //                }
    //                else
    //                {
    //                    map.SetTile(tiles[i], tile);
    //                }


    //                //Right
    //                if ((map.GetTile(tiles[i] + new Vector3Int(1, 0, 0)) == null &&
    //                    main.GetTile(tiles[i] + new Vector3Int(1, 0, 0)) == null &&
    //                    main.GetTile(tiles[i] + new Vector3Int(0, -1, 0)) != null &&
    //                    main.GetTile(tiles[i] + new Vector3Int(1, -1, 0)) != null))
    //                {
    //                    map2.SetTile(tiles[i] + new Vector3Int(1, 0, 0), left);
    //                }
    //                if (map.GetTile(tiles[i] + new Vector3Int(-1, 1, 0)) != null)
    //                {
    //                    map2.SetTile(tiles[i] + new Vector3Int(0, 1, 0), left);
    //                }

    //                if (main.GetTile(tiles[i] + new Vector3Int(1, -1, 0)) == null &&
    //                    map2.GetTile(tiles[i] + new Vector3Int(1, 0, 0)) == null &&
    //                    map2.GetTile(tiles[i] + new Vector3Int(0, 1, 0)) == null &&
    //                    map.GetTile(tiles[i] + new Vector3Int(0, 1, 0)) == null)
    //                {
    //                    map.SetTile(tiles[i], left);
    //                }
    //                else
    //                {
    //                    map.SetTile(tiles[i], tile);
    //                }

    //            }
    //            for (int i = 0; i < tiles.Count; i++)
    //            {
    //                if (tiles[tiles.Count - 1].y < tiles[i].y)
    //                {
    //                    map.SetTile(tiles[i], null);
    //                    tiles.RemoveAt(i);
    //                    break;
    //                }


    //                if (main.GetTile(tiles[i] + new Vector3Int(0, -1, 0)) == null && map.GetTile(tiles[i] + new Vector3Int(0, -1, 0)) == null)
    //                {
    //                    map.SetTile(tiles[i], null);
    //                    tiles[i] = tiles[i] + new Vector3Int(0, -1, 0);
    //                    map.SetTile(tiles[i], tile);
    //                    break;
    //                }
    //                if (tiles.Count < litles)
    //                {
    //                    if (main.GetTile(tiles[i] + new Vector3Int(0, -1, 0)) != null && map.GetTile(tiles[i] + new Vector3Int(-1, 0, 0)) == null && main.GetTile(tiles[i] + new Vector3Int(-1, 0, 0)) == null)
    //                    {
    //                        var n = tiles[i] + new Vector3Int(-1, 0, 0);
    //                        tiles.Add(n);
    //                        map.SetTile(n, tile);
    //                        break;
    //                    }
    //                    if (main.GetTile(tiles[i] + new Vector3Int(0, -1, 0)) != null && map.GetTile(tiles[i] + new Vector3Int(1, 0, 0)) == null && main.GetTile(tiles[i] + new Vector3Int(1, 0, 0)) == null)
    //                    {
    //                        var n = tiles[i] + new Vector3Int(1, 0, 0);
    //                        tiles.Add(n);
    //                        map.SetTile(n, tile);
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }


    #endregion
}

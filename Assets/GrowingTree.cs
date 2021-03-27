using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrowingTree : MonoBehaviour
{
    public GameObject[] treeBiom0;
    public GameObject[] treeBiom1;
    public GameObject[] treeBiom2;

    public TileBase[] canBlocks;

    public float growProgress = 0;
    public float growSpeed = 0;
    public float growEnd = 0;

    public int biom = -1;

    public List<int> CanBiomes;

    public bool canGrow;

    private void Start()
    {
        biom = FindObjectOfType<WorldGenerator>().chunks[GetComponent<Entity>().chunk.x][GetComponent<Entity>().chunk.y].biom;
        canGrow = CanBiomes.Contains(biom);
        canGrow = canBlocks.Contains(FindObjectOfType<WorldGenerator>().main.GetTile(FindObjectOfType<WorldGenerator>().main.WorldToCell(Vector3Int.CeilToInt(new Vector3(transform.position.x, transform.position.y) + new Vector3(0,-1.5f)))));
        print(FindObjectOfType<WorldGenerator>().main.GetTile(FindObjectOfType<WorldGenerator>().main.WorldToCell(Vector3Int.CeilToInt(new Vector3(transform.position.x, transform.position.y) + new Vector3(0, -1.5f)))).name);
        var treesgrow = FindObjectsOfType<GrowingTree>().ToList().FindAll(x => Vector2.Distance(transform.position, x.transform.position) <= 5 && x != this);
        print(treesgrow.Count);
        var treesfull = FindObjectsOfType<Tree>().ToList().FindAll(x => Vector2.Distance(transform.position, x.transform.position) <= 5);
        print(treesfull.Count);
        canGrow = treesgrow.Count == 0 && treesfull.Count == 0;
    }

    private void Update()
    {
        if (canGrow)
        {
            if (growProgress < growEnd)
            {
                growProgress += growSpeed * Time.deltaTime;
            }
            if (growProgress >= growEnd)
            {
                if (biom == 0)
                {
                    Instantiate(treeBiom0[Random.Range(0, treeBiom0.Length)].gameObject, transform.position + new Vector3(0.5f,0,0), Quaternion.identity);
                    Destroy(gameObject);
                }
                if (biom == 1)
                {
                    Instantiate(treeBiom1[Random.Range(0, treeBiom1.Length)].gameObject, transform.position + new Vector3(0.5f,0,0), Quaternion.identity);
                    Destroy(gameObject);
                }
                if (biom == 2)
                {
                    Instantiate(treeBiom2[Random.Range(0, treeBiom2.Length)].gameObject, transform.position + new Vector3(0.5f, 0, 0), Quaternion.identity);
                    Destroy(gameObject);
                }
            }
        }
    }

}

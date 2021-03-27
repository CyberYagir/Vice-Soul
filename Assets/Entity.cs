using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public string entityID;
    public float digTime;
    public int digLvl;
    public Vector2Int chunk;
    public string data;
    private void Start()
    {
        var mg = FindObjectOfType<WorldGenerator>();
        chunk = mg.GetChunkByPos(transform);
        transform.parent = mg.chunks[chunk.x][chunk.y].objects.transform;
    }
}

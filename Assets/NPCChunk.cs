using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCChunk : MonoBehaviour
{
    public NPC npc;
    public WorldGenerator worldGenerator;
    IEnumerator enumerator;
    private void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        npc = GetComponent<NPC>();
        enumerator = loop();

        StartCoroutine(enumerator);
    }

    private void FixedUpdate()
    {
        if (enumerator == null)
        {
            enumerator = loop();
            StartCoroutine(enumerator);
        }
    }
    IEnumerator loop()
    {
        while (true)
        {
        yield return new WaitForSeconds(1);
        npc.enabled = worldGenerator.GetChunkByPos(transform) == worldGenerator.playerChunkPos;

        }
    }
}

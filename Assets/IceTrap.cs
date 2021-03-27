using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTrap : MonoBehaviour
{
    public GameObject item;
    public List<GameObject> items;
    void Start()
    {
        if (FindObjectOfType<WorldGenerator>().back.GetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y, 0)) == null)
        {
            Destroy(gameObject);
        }
        StartCoroutine(loop());
    }

    IEnumerator loop()
    {
        int i = 0;
        while (true)
        {
            i++;
            if (i > 100)
            {
                yield break;
            }
            if (FindObjectOfType<WorldGenerator>().main.GetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y + items.Count, 0)) != null)
            {
                yield break;
            }
            GameObject it = Instantiate(item, transform);
            it.transform.position = transform.position + new Vector3(0, items.Count, 0);
            it.SetActive(true);
            items.Add(it);
        }
    }
}

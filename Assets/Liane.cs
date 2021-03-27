using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Liane : MonoBehaviour
{
    public GameObject item;
    public Sprite end;
    public int length;
    public List<GameObject> items;

    public Tilemap main;

    public bool fall = false;

    public Rigidbody2D rb;

    public GameObject drop;

    private void Start()
    {
        if (FindObjectOfType<WorldGenerator>().back.GetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y, 0)) == null)
        {
            Destroy(gameObject);
        }
        GetComponent<SpriteRenderer>().enabled = false;
        main = FindObjectOfType<WorldGenerator>().main;
        length = Random.Range(5, 10);
        for (int i = 0; i < length; i++)
        {
            if (main.GetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y + -i, 0)) == null)
            {
                GameObject it = Instantiate(item.gameObject, transform.position + new Vector3(0, -i, 0), Quaternion.identity);
                it.SetActive(true);
                it.transform.parent = transform;
                items.Add(it);
            }
        }
        items[items.Count- 1].GetComponent<SpriteRenderer>().sprite = end;
    }
    private void FixedUpdate()
    {
        if (fall == false)
        {
            if (main.GetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y + 1, 0)) == null)
            {
                var p = GetComponent<BoxCollider2D>();
                p.size = new Vector2(p.size.x/2, p.size.y/2);
                rb.bodyType = RigidbodyType2D.Dynamic;
                fall = true;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (fall == true)
        {
            Item it = FindObjectOfType<WorldManager>().GetItemBySecondName("_Liana_");
            it.value = Random.Range(2, 20);
            GameObject g = Instantiate(drop, transform.position + new Vector3(0,0.55f,0), Quaternion.identity);
            g.GetComponent<Drop>().item = it;
            g.GetComponent<Drop>().Init();
            Destroy(gameObject);
        }
    }
}

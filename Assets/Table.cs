using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public string tableType;
    public Vector2Int size;
    public bool canDrop = true;
    WorldManager manager;
    private void Start()
    {
        manager = FindObjectOfType<WorldManager>();
        StartCoroutine(check());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponentInParent<PlayerStats>().localPlayer.craftingTables.Add(tableType);
        }
    }
    IEnumerator check()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (gameObject.active)
            {
                bool all = true;
                for (int i = 0; i < size.x + 1; i++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(i, -0.5f), Vector2.zero);
                    if (hit.collider == null)
                    {
                        all = false;
                        break;
                    }
                }
                if (!all && canDrop)
                {
                    Drop gm = Instantiate(manager.drop, transform.position, Quaternion.identity).GetComponent<Drop>();
                    gm.item = manager.GetItemBySecondName(GetComponent<Entity>().entityID);
                    gm.item.value = 1;
                    gm.Init();
                    Destroy(gameObject);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponentInParent<PlayerStats>().localPlayer.craftingTables.Remove(tableType);
        }
    }
}

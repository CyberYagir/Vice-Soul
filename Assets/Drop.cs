using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public Item item;



    private void Start()
    {
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), FindObjectOfType<Player2D>().GetComponent<CapsuleCollider2D>());
        GetComponent<Rigidbody2D>().velocity = (new Vector2(Random.Range(-5, 5), 0));
        StartCoroutine(wait());
        var drops = FindObjectsOfType<Drop>().ToList().FindAll(x => Vector2.Distance(transform.position, x.transform.position) < 5);
        for (int i = 0; i < drops.Count; i++)
        {
            if (drops[i].item.secondName == item.secondName)
            {
                int n = item.value;
                for (int h = 0; h < n; h++)
                {
                    if (drops[i].item.value >= drops[i].item.maxValue) break;
                    if (item.value > 0) {
                        drops[i].item.value++;
                        item.value--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        if (item.value == 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        GetComponent<SpriteRenderer>().sprite = item.icon;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        if (item.type == Item.itemType.Table)
        {
            item.icon =  Sprite.Create(sprite.texture, sprite.rect, Vector2.zero);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && collision.GetComponent<PlayerCollider>() == null && collision is BoxCollider2D == false)
        {
            if (FindObjectOfType<PlayerStats>().localPlayer.inventory.Count < FindObjectOfType<PlayerStats>().localPlayer.inventorySize)
            {
                var player = FindObjectOfType<PlayerUI>();
                player.AddItem(item);
                print("ADD");
                Destroy(gameObject);
                return;
            }
        }
    }

    IEnumerator wait()
    {
        for(float i = 1; i > 0; i -= 0.1f) {
            yield return new WaitForSeconds(6);
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSharp : MonoBehaviour
{
    public LayerMask layer;
    public LayerMask layer1;
    public int damage;

    public void Start()
    {
        StartCoroutine(loop());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.transform.parent != null)
            {
                collision.transform.parent.GetComponent<PlayerStats>().TakeDamage((float)damage);
                if (collision.transform.parent.position.x < transform.position.x)
                {
                    collision.transform.parent.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1000, 200));
                }
                if (collision.transform.parent.position.x > transform.position.x)
                {
                    collision.transform.parent.GetComponent<Rigidbody2D>().AddForce(new Vector2(1000, 200));
                }
            }
        }
    }

    IEnumerator loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.5f, -0.51f), Vector2.down, 0.25f, layer);
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(0.5f, -0.51f), Vector2.down, 0.25f, layer1);
            if (hit.collider == null && hit1.collider == null)
            {
                GameObject it = Instantiate(FindObjectOfType<WorldManager>().drop);
                it.transform.position = transform.position;
                it.GetComponent<Drop>().item = FindObjectOfType<WorldManager>().GetItemBySecondName("_Sharp_");
                it.GetComponent<Drop>().Init();
                Destroy(gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public int damage;
    public float startForce;
    public Vector3 pos;
    public Rigidbody2D rb;
    public bool connected;
    void Start()
    {
        pos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        Destroy(gameObject, 10);
    }
    private void FixedUpdate()
    {
        if (connected == false)
        {
            //transform.right = pos;
            rb.AddRelativeForce(Vector2.right * startForce * Time.deltaTime, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.transform.tag == "Mob" || collision.transform.gameObject.layer != 8) && collision.transform.tag != "Player" && connected == false && (collision.isTrigger == false || collision.transform.tag == "Mob"))
        {
            connected = true;
            rb.freezeRotation = true;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            if (rb != null)
            {
                Destroy(rb);
            }
            transform.parent = collision.transform;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponentInChildren<TrailRenderer>().enabled = false;
            if (collision.tag == "Mob")
            {
                collision.GetComponentInParent<MobStats>().hp -= damage;
            }
        }
    }
}

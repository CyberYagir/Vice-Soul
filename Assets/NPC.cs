using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Info")]
    public string NPCName;
    public string NPCDesc;
    public Sprite icon;
    public GameObject canvas;
    [Header("NPC")]
    [Space]
    public int Move;
    public float speed, jumpForce;
    public Rigidbody2D rb;
    public LayerMask layer;
    public bool start;
    public Transform player;
    public bool firstStart = true;
    public GameObject right;
    float startX;
    [Space]
    public TextTranslator name;
    public TextTranslator desc;

    private void Start()
    {
        startX = transform.localScale.x;
        StartCoroutine(move());
        StartCoroutine(jump());
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (player.GetComponent<CapsuleCollider2D>() != null)
        {
            Physics2D.IgnoreCollision(player.GetComponent<CapsuleCollider2D>(), transform.GetComponent<CapsuleCollider2D>(), true);
        }
        if (player.GetComponent<BoxCollider2D>() != null)
        {
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), transform.GetComponent<CapsuleCollider2D>(), true);
        }
        if (firstStart)
        {
            transform.position = new Vector2(player.transform.position.x + 5, -100);
        }
        right = new GameObject();
        right.transform.SetParent(transform);
        right.name = "Hitter";
        right.transform.localPosition = new Vector3(0.5f, 0, 0);
    }
    void Update()
    {
        var cl = GetComponent<CapsuleCollider2D>();
        Physics2D.IgnoreCollision(player.GetComponent<Hook>().capsule, cl, true );
        if (!start)
        {
            if (player.GetComponent<Player2D>().startCollide == true)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, (transform.localScale.y / 2) - 0.1f) + new Vector3(0, 100), Vector2.down, Mathf.Infinity, layer);
                Debug.DrawRay(transform.InverseTransformPoint(0.5f, 0, 0), Vector2.down);
                if (hit.collider != null)
                {
                    rb.velocity = Vector2.zero;
                    transform.position = (Vector3)hit.point + new Vector3(0, (transform.localScale.y / 2) + 0.1f, transform.position.z);
                    start = true;
                    return;
                }
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, (transform.localScale.y / 2) - 0.1f), Vector2.down, Mathf.Infinity, layer);
            RaycastHit2D hit2 = Physics2D.Raycast(right.transform.position, Vector3.down, 20, layer);
            Debug.DrawRay(transform.InverseTransformPoint(0.5f, 0, 0), Vector2.down);
            if (hit.collider != null && hit2.collider != null)
            {
                if (Move > 0)
                {
                    rb.AddRelativeForce(Vector3.right * speed * Time.deltaTime, ForceMode2D.Force);
                    transform.localScale = new Vector3(startX, transform.localScale.y, 1);
                }
                if (Move < 0)
                {
                    rb.AddRelativeForce(Vector3.left * speed * Time.deltaTime, ForceMode2D.Force);
                    transform.localScale = new Vector3(-startX, transform.localScale.y, 1);
                }
            }
            else
            {
                Move = -Move;
            }
        }
        
    }
    IEnumerator jump()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (this.enabled == true)
            {
                if (Move != 0)
                {
                    if (rb.velocity.x == 0)
                    {
                        rb.AddRelativeForce(Vector3.up * jumpForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
    IEnumerator move()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Move = Random.Range(-3, 3);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            canvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            canvas.SetActive(false);
        }
    }
}

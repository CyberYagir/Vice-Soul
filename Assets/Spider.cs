using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spider : MonoBehaviour
{
    public float speed;
    public Tilemap back;
    public MobStats mobStats;
    public float agrDistace, unAgrDistace;
    public bool isAgr;
    public Transform player;
    public bool dead;
    public Transform point;
    public float punchForce;
    private bool attacked, last;
    public GameObject deadParticles;
    public Animator animator;
    public string walk, idle, attack;
    private void Start()
    {
        player = FindObjectOfType<Player2D>().transform;
        back = FindObjectOfType<WorldGenerator>().back;

        speed = mobStats.speed;

        if( back.GetTile(Vector3Int.CeilToInt(transform.position)) == null)
        {
            Destroy(gameObject);
        }

    }


    private void Update()
    {
        if (!dead)
        {
            if (mobStats.hp <= 0)
            {
                if (last == false)
                {
                    GameObject g = Instantiate(deadParticles.gameObject, transform.position, Quaternion.identity);
                    g.SetActive(true);
                    dead = true;
                    Destroy(gameObject, 0.2f);
                    last = true;
                }
            }


            if (Vector3.Distance(player.transform.position, transform.position) <= agrDistace)
            {
                isAgr = true;
            }
            if (Vector3.Distance(player.transform.position, transform.position) >= unAgrDistace)
            {
                isAgr = false;
            }
            if (GetComponent<Rigidbody2D>().velocity.magnitude > 2)
            {
                animator.Play(walk);
            }
            else
            {
                animator.Play(idle);
            }
            if (isAgr == true)
            {
                Vector3 diff = player.transform.position - transform.position;
                diff.Normalize();
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

                if (back.GetTile(Vector3Int.CeilToInt(point.transform.position)) != null)
                {
                    GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.right * speed * Time.deltaTime);
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
        }
    }


    IEnumerator wait()
    {
        yield return new WaitForSeconds(mobStats.attackCooldown);
        attacked = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (attacked == false)
            {
                if (player == null)
                {
                    player = FindObjectOfType<Player2D>().transform;
                }

                player.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * punchForce * Time.deltaTime, ForceMode2D.Impulse);
                if (player.position.x < transform.position.x)
                {
                    player.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.left * punchForce * Time.deltaTime, ForceMode2D.Impulse);
                }
                if (player.position.x > transform.position.x)
                {
                    player.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.right * punchForce * Time.deltaTime, ForceMode2D.Impulse);
                }
                player.GetComponent<PlayerStats>().TakeDamage(mobStats.attack);
                attacked = true;
                StartCoroutine(wait());
            }
        }
    }
}

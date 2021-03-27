using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float speed, jumpForce, punchForce;
    public Rigidbody2D rb;

    public Transform player;

    public float agrDistace, unAgrDistace;
    public bool isAgr;

    public bool isJumped, jumped;

    public float jumpFeetsDist, legs, jumpcooldown;

    public Animator animator;

    public MobStats stats;

    public bool attacked;
    public GameObject deadParticles;
    public bool dead, last;

    public string Jump, Walk, Idle, attack;

    public float xOffcet;
    public GameObject rig;
    public LayerMask mask;

    private void Start()
    {
        stats.Start();
        player = FindObjectOfType<Player2D>().transform;
        jumpForce = stats.jumpForce;
        speed = stats.speed;
        Physics2D.IgnoreCollision(player.GetComponent<CapsuleCollider2D>(), transform.GetComponent<BoxCollider2D>(), true);
        Physics2D.IgnoreCollision(player.GetComponent<CapsuleCollider2D>(), transform.GetComponent<CircleCollider2D>(), true);

        var NPCs = GameObject.FindGameObjectsWithTag("NPC");
        for (int i = 0; i < NPCs.Length; i++)
        {
            Physics2D.IgnoreCollision(transform.GetComponent<BoxCollider2D>(), NPCs[i].GetComponent<Collider2D>(), true);
            Physics2D.IgnoreCollision(transform.GetComponent<CircleCollider2D>(), NPCs[i].GetComponent<Collider2D>(), true);
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(stats.attackCooldown);
        attacked = false;
    }
    IEnumerator waitJump()
    {
        yield return new WaitForSeconds(jumpcooldown);
        jumped = false;
    }
    private void FixedUpdate()
    {
        if (!dead)
        {
            //Debug.DrawRay(new Vector2(transform.position.x - xOffcet, transform.position.y - transform.localScale.y - legs), -Vector3.up);
            //Debug.DrawRay(new Vector2(transform.position.x + xOffcet, transform.position.y - transform.localScale.y - legs), -Vector3.up);
            var dist = Vector3.Distance(player.transform.position, transform.position);
            if (dist <= agrDistace)
            {
                isAgr = true;
            }
            if (dist >= unAgrDistace)
            {
                isAgr = false;
            }
            if (isAgr == true)
            {
                //if (jumped == false)
                //{
                //    if (isJumped)
                //    {
                //    }
                //    if (!isJumped)
                //    {
                //        if (player.position.x < transform.position.x)
                //        {
                //            rb.AddRelativeForce(Vector2.left * speed * Time.deltaTime, ForceMode2D.Force);
                //        }
                //        if (player.position.x > transform.position.x)
                //        {
                //            rb.AddRelativeForce(Vector2.right * speed * Time.deltaTime, ForceMode2D.Force);
                //        }
                //        
                //    }
                //}
                if (player.position.x < transform.position.x)
                {
                    rig.transform.localScale = new Vector3(-Mathf.Abs(rig.transform.localScale.x), rig.transform.localScale.y, rig.transform.localScale.z);
                    rb.AddRelativeForce(Vector2.left * speed * Time.deltaTime, ForceMode2D.Force);
                }
                if (player.position.x > transform.position.x)
                {
                    rig.transform.localScale = new Vector3(Mathf.Abs(rig.transform.localScale.x), rig.transform.localScale.y, rig.transform.localScale.z);
                    rb.AddRelativeForce(Vector2.right * speed * Time.deltaTime, ForceMode2D.Force);
                }
                if (jumped == false)
                {
                    if (Mathf.Abs(rb.velocity.x) <= 0f)
                    {
                        rb.AddRelativeForce(Vector2.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);
                        jumped = true;
                        StartCoroutine(waitJump());
                    }
                }
            }
            if (isJumped)
            {
                animator.Play(Jump);
            }
            if (stats.hp <= 0)
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
            if (!isJumped && Mathf.Abs(rb.velocity.x) > 2 && !attacked )
            {
                animator.Play(Walk);
            }
            if (!isJumped && Mathf.Abs(rb.velocity.x) < 2 && !attacked)
            {
                if (Idle != "")
                {
                    animator.Play(Idle);
                }
            }
            if (attacked)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.Play(attack);
            }
            if (isAgr)
            {
                RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - xOffcet, transform.position.y - transform.localScale.y - legs), -Vector2.up, Mathf.Infinity, mask);
                RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + xOffcet, transform.position.y - transform.localScale.y - legs), -Vector2.up, Mathf.Infinity, mask);

                //Debug.DrawRay(new Vector2(transform.position.x + xOffcet, transform.position.y - transform.localScale.y - legs), -Vector2.up, Color.red);

                if (hitLeft.collider != null || hitRight.collider != null)
                {
                    if ((Vector2.Distance(hitLeft.point, transform.position) <= jumpFeetsDist && hitLeft.transform.gameObject.layer != 8) ||
                        (Vector2.Distance(hitRight.point, transform.position) <= jumpFeetsDist && hitRight.transform.gameObject.layer != 8))
                    {
                        //if (hitLeft.collider != null)
                        //{
                        //    print("HL: " + hitLeft.transform.name);
                        //}
                        //if (hitRight.collider != null)
                        //{
                        //    print("HR: " + hitRight.transform.name);
                        //}
                        isJumped = false;
                    }
                    else
                    {
                        isJumped = true;
                    }

                }
                else
                {
                    isJumped = false;
                }
            }
            else
            {
                isJumped = false;
            }
        }
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
                animator.Play(attack);
                player.GetComponent<PlayerStats>().TakeDamage(stats.attack);
                attacked = true;
                StartCoroutine(wait());
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumped = false;
    }
}

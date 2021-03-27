using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    public float agrDist, unAgrDist;
    public Vector2 newPos;
    public float range;

    public float maxY;

    public bool attacked;
    public float punchForce;
    public bool isAgr;

    public bool dead;
    public float oldHp;
    public bool attackedPlayer;
    public MobStats mobStats;
    
    Player2D player;
    private bool last;
    public GameObject deadParticles;
    private void Start()
    {
        oldHp = mobStats.hp;
        player = FindObjectOfType<Player2D>();
        StopAllCoroutines();
        StartCoroutine(newPosCalc());
    }

    private void Update()
    {
        if (dead == false)
        {
            var npcs = FindObjectsOfType<NPC>();
            for (int i = 0; i < npcs.Length; i++)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), npcs[i].GetComponent<CapsuleCollider2D>());
            }
            if (oldHp != mobStats.hp)
            {
                StartCoroutine(gravity());
                oldHp = mobStats.hp;
            }
            if (Vector2.Distance(player.transform.position, transform.position) <= agrDist)
            {
                isAgr = true;
            }
            if (isAgr == true)
            {
                if (Vector2.Distance(player.transform.position, transform.position) >= unAgrDist)
                {
                    isAgr = false;
                }
            }
            if (isAgr)
            {
                if (attacked == false)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * mobStats.speed);

                    if (Vector2.Distance(player.transform.position, transform.position) <= 1.35)
                    {
                        if (player == null)
                        {
                            player = FindObjectOfType<Player2D>();
                        }
                        player.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * punchForce * Time.deltaTime, ForceMode2D.Impulse);
                        if (player.transform.position.x < transform.position.x)
                        {
                            player.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.left * punchForce * Time.deltaTime, ForceMode2D.Impulse);
                        }
                        if (player.transform.position.x > transform.position.x)
                        {
                            player.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.right * punchForce * Time.deltaTime, ForceMode2D.Impulse);
                        }
                        player.transform.GetComponent<PlayerStats>().TakeDamage(mobStats.attack);

                        StartCoroutine(wait());
                        attacked = true;
                    }
                }
                else
                {
                    transform.position = Vector2.Lerp(transform.position, newPos, Time.deltaTime * mobStats.speed);
                }
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, newPos, Time.deltaTime * mobStats.speed);
            }
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
        }
    }
    IEnumerator gravity()
    {
        GetComponent<Rigidbody2D>().gravityScale = 5;
        yield return new WaitForSeconds(2);
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }
    IEnumerator wait()
    {
        
        yield return new WaitForSeconds(mobStats.attackCooldown);
        attacked = false;
    }
    IEnumerator newPosCalc()
    {
        while (true)
        {
            if (isAgr == false)
            {
                newPos = (Vector2)transform.position +  new Vector2(Random.Range(-range, range), Random.Range(-range, range));
                if (newPos.y > maxY)
                {
                    newPos = new Vector2(newPos.x, maxY);
                }
            }
            else
            {
                newPos = (Vector2)transform.position + new Vector2(Random.Range(-range, range), Random.Range(-range, range));
            }
            yield return new WaitForSeconds(1);
        }
    }
}

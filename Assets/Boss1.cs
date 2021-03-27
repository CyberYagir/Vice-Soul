using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    public float speed;
    public Transform player;
    public Animator animator;
    public Vector3 pos;
    public IEnumerator enumerator;
    public Vector3 startSize;
    public bool attack, fire;
    public float attackCoolDown, attackWaitNext;
    public LineRenderer line;
    public LayerMask mask;
    public int damage;
    public MobStats stats;
    public TMPro.TMP_Text text;
    public bool dead;
    
    float startOrthSize;
    Vector2Int vector;

    private void Start()
    {
        line.enabled = false;
        startOrthSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = 30;
        player = FindObjectOfType<Player2D>().transform;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            vector = FindObjectOfType<ShadowTileMap>().size;
            FindObjectOfType<ShadowTileMap>().size = new Vector2Int(58, 35);
        }

        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), transform.GetComponentInChildren<Collider2D>(), true);
        startSize = transform.localScale;
        enumerator = wait();
        StartCoroutine(enumerator);
    }

    public void Update()
    {
        FindObjectOfType<TimeCycle>().timeLine = 0;
        var p = player.GetComponent<PlayerStats>().localPlayer;
        if (p.health <= 0)
        {
            pos = new Vector3(pos.x, -100, pos.z);
            transform.position = Vector3.Lerp(transform.position, pos, 1 * Time.deltaTime);
            Camera.main.orthographicSize = startOrthSize;
            if (FindObjectOfType<ShadowTileMap>() != null)
            {
                FindObjectOfType<ShadowTileMap>().size = vector;
            }
            Destroy(gameObject, 2);
            return;
        }

        if (dead == false)
        {
            Camera.main.orthographicSize = 30;
            if (FindObjectOfType<ShadowTileMap>() != null)
            {
                FindObjectOfType<ShadowTileMap>().size = new Vector2Int(58, 35);
            }


            text.text = "Rorak - " + stats.hp + "/" + stats.maxHp;

            if (enumerator == null)
            {
                StopAllCoroutines();
                enumerator = wait();
                StartCoroutine(enumerator);
            }
            transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
            if (player.transform.position.x < transform.position.x)
            {
                transform.localScale = startSize;
            }
            if (player.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-startSize.x, startSize.y, startSize.z);
            }

            if (attack == false)
            {
                animator.Play("SatanCircle");
                line.gameObject.SetActive(true);
                StartCoroutine(attackWait());
                attack = true;
                fire = true;
            }
            if (fire == true)
            {
                RaycastHit2D hit = Physics2D.Raycast(line.transform.position, -line.transform.right, Mathf.Infinity, mask);
                if (transform.localScale.x > 0)
                {
                    hit = Physics2D.Raycast(line.transform.position, -line.transform.right, Mathf.Infinity, mask);
                }
                if (transform.localScale.x < 0)
                {
                    hit = Physics2D.Raycast(line.transform.position, line.transform.right, Mathf.Infinity, mask);
                }

                line.SetPosition(1, line.transform.position);
                if (hit.collider != null)
                {
                    line.gameObject.SetActive(true);
                    line.SetPosition(0, line.transform.position);
                    if (hit.transform.tag == "Player")
                    {
                        hit.transform.GetComponent<PlayerStats>().TakeDamage(damage);
                    }
                }
                else
                {
                    line.gameObject.SetActive(true);
                }
            }
        }
        if (stats.hp <= 0)
        {
            if (dead == false)
            {
                speed = 0;
                animator.Play("SatanDeath"); line.gameObject.SetActive(false);
                StartCoroutine(deadWait());
                dead = true;
            }
        }
    }
    IEnumerator deadWait()
    {
        yield return new WaitForSeconds(25f);
        Camera.main.orthographicSize = startOrthSize;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            FindObjectOfType<ShadowTileMap>().size = vector;
        }
        FindObjectOfType<WorldManager>().firstBoss = true;
        var npcs = FindObjectsOfType<NPC>();
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i].NPCName == "Тьма вне плоти")
            {
                GameObject gm = Instantiate(FindObjectOfType<WorldManager>().NPCs[1].gameObject);
                
                gm.GetComponent<NPC>().firstStart = true;
                Destroy(npcs[i].gameObject);
                break;
            }
        }
        Destroy(gameObject);
    }

    public void FixedUpdate()
    {
        if (fire == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(line.transform.position, -line.transform.right, Mathf.Infinity, mask);
            RaycastHit2D hit1 = Physics2D.Raycast(line.transform.position + new Vector3(0, 1), -line.transform.right, Mathf.Infinity, mask);
            RaycastHit2D hit2 = Physics2D.Raycast(line.transform.position + new Vector3(0, -1), -line.transform.right, Mathf.Infinity, mask);
            if (transform.localScale.x > 0)
            {
                hit = Physics2D.Raycast(line.transform.position, -line.transform.right, Mathf.Infinity, mask);
                hit1 = Physics2D.Raycast(line.transform.position + new Vector3(0, 1), -line.transform.right, Mathf.Infinity, mask);
                hit2 = Physics2D.Raycast(line.transform.position + new Vector3(0, -1), -line.transform.right, Mathf.Infinity, mask);
            }
            if (transform.localScale.x < 0)
            {
                hit = Physics2D.Raycast(line.transform.position, line.transform.right, Mathf.Infinity, mask);
                hit1 = Physics2D.Raycast(line.transform.position + new Vector3(0, 1), line.transform.right, Mathf.Infinity, mask);
                hit2 = Physics2D.Raycast(line.transform.position + new Vector3(0, -1), line.transform.right, Mathf.Infinity, mask);
            }

            line.SetPosition(1, line.transform.position);
            if (hit.collider != null)
            {
                if (hit.transform.tag == "Player")
                {
                    hit.transform.GetComponent<PlayerStats>().TakeDamage(damage);
                    return;
                }
            }
            if (hit1.collider != null)
            {
                if (hit1.transform.tag == "Player")
                {
                    hit1.transform.GetComponent<PlayerStats>().TakeDamage(damage);
                    return;
                }
            }
            if (hit2.collider != null)
            {
                if (hit2.transform.tag == "Player")
                {
                    hit2.transform.GetComponent<PlayerStats>().TakeDamage(damage);
                    return;
                }
            }
        }
    }
    IEnumerator attackWait()
    {
        yield return new WaitForSeconds(attackCoolDown);
        line.gameObject.SetActive(false);
        fire = false;
        yield return new WaitForSeconds(attackWaitNext);
        attack = false;
    }

    IEnumerator wait()
    {
        while (true)
        {
            yield return new WaitForSeconds(4);
            pos = player.transform.position + new Vector3(Random.Range(-25, 25), Random.Range(0, 30),0);
        }
    }

}

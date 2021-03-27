using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public float speed, jump;
    public Rigidbody2D rb;
    public GameObject player;
    public float maxDist;
    public float magn;

    [Space]
    public Transform shootPoint;
    public LineRenderer lineRenderer;
    public Transform inPoint;
    [Space]
    public LayerMask mask;
    [Space]
    public TMPro.TMP_Text text;
    [Space]
    public MobStats stats;
    float startOrthSize;
    public Vector2Int vector;
    public bool canShoot = true;
    public bool isJump = false, dead = false, anim = false;
    int noJumpCount = 0;
    public bool moveToPlayer = false;
    public List<BoxCollider2D> collders;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player2D>().gameObject;

        var colls = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colls.Length; i++)
        {
            Physics2D.IgnoreCollision(colls[i], player.GetComponent<CapsuleCollider2D>(),true);
            Physics2D.IgnoreCollision(colls[i], player.GetComponent<BoxCollider2D>(),true);
        }
        for (int i = 0; i < collders.Count; i++)
        {
            Physics2D.IgnoreCollision(collders[i], player.GetComponent<CapsuleCollider2D>(),true);
            Physics2D.IgnoreCollision(collders[i], player.GetComponent<BoxCollider2D>(),true);
        }


        startOrthSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = 30;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            vector = FindObjectOfType<ShadowTileMap>().size;
            FindObjectOfType<ShadowTileMap>().size = new Vector2Int(58, 35);
        }
        
        StartCoroutine(mirror());

        shootPoint.transform.parent = null;
        StartCoroutine(coolDown());

    }
    IEnumerator coolDown()
    {
        while (true)
        {
            canShoot = true;
            yield return new WaitForSeconds(stats.attackCooldown);
            canShoot = false; 
            yield return new WaitForSeconds(stats.attackCooldown/4);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Camera.main.orthographicSize = 30;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            FindObjectOfType<ShadowTileMap>().size = new Vector2Int(58, 35);
        }

        if (shootPoint.transform.parent != null)
        {
            shootPoint.transform.parent = null;
        }
        text.text = "Logan - " + stats.hp + "/" + stats.maxHp;
        Vector3 dir = player.transform.position - shootPoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        shootPoint.rotation = Quaternion.Lerp(shootPoint.rotation, Quaternion.AngleAxis(angle, Vector3.forward), 2f * Time.deltaTime);
        //shootPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    void Update()
    {
        FindObjectOfType<TimeCycle>().timeLine = 0;
        if (dead == false)
        {
            if (player.GetComponent<PlayerStats>().localPlayer.health <= 0)
            {
                GetComponent<Animator>().Play("Jump");
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 200), 2 * Time.deltaTime);
                if (transform.position.y > 80)
                {
                    Destroy(shootPoint.gameObject);
                    Destroy(gameObject);
                }
                return;
            }
            shootPoint.position = inPoint.position;
            if (moveToPlayer == false)
            {
                if (canShoot)
                {
                    RaycastHit2D hit;
                    RaycastHit2D hit1;
                    RaycastHit2D hit2;
                    lineRenderer.SetPosition(0, shootPoint.position);

                    
                    hit = Physics2D.Raycast(shootPoint.position, shootPoint.right, Mathf.Infinity, mask);
                    hit1 = Physics2D.Raycast(shootPoint.position + new Vector3(0,1,0), shootPoint.right, Mathf.Infinity, mask);
                    hit2 = Physics2D.Raycast(shootPoint.position + new Vector3(0, -1, 0), shootPoint.right, Mathf.Infinity, mask);
                    Debug.DrawLine(shootPoint.position, hit.point);
                    Debug.DrawLine(shootPoint.position, hit1.point);
                    Debug.DrawLine(shootPoint.position, hit2.point);


                    if (hit.collider != null)
                    {
                        if (hit.collider.transform.tag == "Player")
                        {
                            if (player.GetComponent<PlayerUI>().pause == false)
                            {
                                player.GetComponent<PlayerStats>().TakeDamage(5);
                            }
                        }
                        lineRenderer.SetPosition(1, (Vector2)hit.point);
                    }
                    else
                    {
                        lineRenderer.SetPosition(1, shootPoint.transform.position + shootPoint.TransformVector((shootPoint.right * 1000)));
                    }
                    if (hit1.collider != null)
                    {
                        if (hit1.collider.transform.tag == "Player")
                        {
                            if (player.GetComponent<PlayerUI>().pause == false)
                            {
                                player.GetComponent<PlayerStats>().TakeDamage(5);
                            }
                        }
                    }
                    if (hit2.collider != null)
                    {
                        if (hit2.collider.transform.tag == "Player")
                        {
                            if (player.GetComponent<PlayerUI>().pause == false)
                            {
                                player.GetComponent<PlayerStats>().TakeDamage(5);
                            }
                        }
                    }
                }
                else
                {
                    lineRenderer.SetPosition(0, Vector2.zero);
                    lineRenderer.SetPosition(1, Vector2.zero);
                }
                var npcs = FindObjectsOfType<NPC>();
                for (int i = 0; i < npcs.Length; i++)
                {
                    Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), npcs[i].GetComponent<CapsuleCollider2D>(), true);
                }

                if (isJump == false)
                {
                    if (Vector2.Distance(transform.position, player.transform.position) >= maxDist)
                    {
                        moveToPlayer = true;
                        canShoot = false;
                        return;
                        int r = Random.Range(-1, 1);
                        transform.position = player.transform.position + new Vector3(20 * (r == 0 || r == 1 ? 1 : -1), 50, 0);
                    }
                    if (player.transform.position.x > transform.position.x)
                    {
                        rb.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
                        //transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    else
                    {
                        rb.MovePosition(transform.position + -Vector3.right * speed * Time.deltaTime);
                        //transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    GetComponent<Animator>().Play("Walk");
                }
                magn = rb.velocity.magnitude;
                if (rb.velocity.magnitude == 0)
                {
                    noJumpCount++;
                    if (noJumpCount >= 100)
                    {
                        isJump = true;
                        GetComponent<Animator>().Play("Jump");
                        transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, -50), 4f);
                        if ((Vector2)transform.position == new Vector2(transform.position.x, -50))
                        {
                            noJumpCount = 0;
                            isJump = false;
                        }
                    }
                }
            }
            else
            {
                GetComponent<Rigidbody2D>().gravityScale = 0;
                transform.position = Vector2.Lerp(transform.position, player.transform.position,1 * Time.deltaTime);
                GetComponent<Animator>().Play("Jump");
                if (Vector2.Distance(transform.position, player.transform.position) < 20)
                {
                    GetComponent<Rigidbody2D>().gravityScale = 5;
                    canShoot = true;
                    moveToPlayer = false;
                }
            }
            if (stats.hp <= 0)
            {
                dead = true;
            }
        }
        else
        {
            shootPoint.gameObject.SetActive(false);
            if (anim == false) {

                GetComponent<Animator>().Play("Death");
                anim = true;
                StartCoroutine(deadWait()); 
            }
        }
    }
    private void OnDestroy()
    {
        Camera.main.orthographicSize = startOrthSize;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            FindObjectOfType<ShadowTileMap>().size = vector;
        }
    }
    IEnumerator deadWait()
    {
        yield return new WaitForSeconds(27f);
        Camera.main.orthographicSize = startOrthSize;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            FindObjectOfType<ShadowTileMap>().size = vector;
        }
        FindObjectOfType<WorldManager>().secondBoss = true;
        var npcs = FindObjectsOfType<NPC>();
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i].NPCName == "Концентрированная тьма вне плоти")
            {
                GameObject gm = Instantiate(FindObjectOfType<WorldManager>().NPCs[2].gameObject);
                gm.GetComponent<NPC>().firstStart = true;
                Destroy(npcs[i].gameObject);
                break;
            }
        }
        GameObject mage = Instantiate(FindObjectOfType<WorldManager>().NPCs[3].gameObject);
        mage.GetComponent<NPC>().firstStart = true;
        Destroy(gameObject);
    }

    IEnumerator mirror()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (player.transform.position.x > transform.position.x)
            {
                //rb.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
               //rb.MovePosition(transform.position + -Vector3.right * speed * Time.deltaTime);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}

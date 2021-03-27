using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss4 : MonoBehaviour
{

    public Vector3 backPos;
    public MobStats mobStats;
    private Player2D player;
    GameObject playerCamera;
    private float startOrthSize;
    [Header("UI")]
    public TMP_Text header;
    public bool isDead, deadIsSatrted;
    private Vector2Int vector;
    public List<BoxCollider2D> collders;
    public float maxDist;
    public GameObject[] bulletSpawns;
    public GameObject bullet, arrow;
    public Vector2 tomovepos;
    public GameObject[] stuffs;
    public Sprite Form2;
    public SpriteRenderer spriteRenderer;
    private void Start()
    {
        player = FindObjectOfType<Player2D>();
        playerCamera = Camera.main.gameObject;
        startOrthSize = Camera.main.orthographicSize;
        StartCoroutine(mirror());

        var colls = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colls.Length; i++)
        {
            Physics2D.IgnoreCollision(colls[i], player.GetComponent<CapsuleCollider2D>(), true);
            Physics2D.IgnoreCollision(colls[i], player.GetComponent<BoxCollider2D>(), true);
        }
        for (int i = 0; i < collders.Count; i++)
        {
            Physics2D.IgnoreCollision(collders[i], player.GetComponent<CapsuleCollider2D>(), true);
            Physics2D.IgnoreCollision(collders[i], player.GetComponent<BoxCollider2D>(), true);
        }
        StartCoroutine(turns());
        StartCoroutine(move());
    }

    private void Update()
    {
        if (!isDead)
        {
            header.text = "Unon - " + mobStats.hp + "/" + mobStats.maxHp;
            Camera.main.orthographicSize = 30;
            if (FindObjectOfType<ShadowTileMap>() != null)
            {
                FindObjectOfType<ShadowTileMap>().size = new Vector2Int(58, 35);
            }
            var npcs = FindObjectsOfType<NPC>();
            for (int i = 0; i < npcs.Length; i++)
            {
                Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), npcs[i].GetComponent<CapsuleCollider2D>(), true);
            }
            if (player.transform.position.y < -250 || player.GetComponent<PlayerStats>().localPlayer.health < 0)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                tomovepos = new Vector2(0, 100);
                transform.position = Vector3.Lerp(transform.position, tomovepos, 5f * Time.deltaTime);
                if (transform.position.y > 80)
                {
                    Destroy(gameObject);
                }
                return;
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = true;
            }
            if (Vector2.Distance(transform.position, player.transform.position) >= maxDist)
            {
                transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0, 10), 5f * Time.deltaTime);
                return;
            }
            for (int g = 0; g < stuffs.Length; g++)
            {
                Vector3 dir = player.transform.position - stuffs[g].transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                stuffs[g].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
            if (mobStats.hp < 2000)
            {
                spriteRenderer.sprite = Form2;
            }
            if (mobStats.hp <= 0)
                isDead = true;
            transform.position = Vector2.MoveTowards(transform.position, tomovepos + new Vector2(0, 10), mobStats.speed * Time.deltaTime);
        }
        else
        {
            if (!deadIsSatrted)
            {
                GetComponent<Animator>().Play("Dead");
                deadIsSatrted = true;
                Destroy(gameObject, 30);
            }
        }
    }
    IEnumerator move()
    {
        while (true)
        {
            tomovepos = player.transform.position;
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator turns()
    {
        while (true)
        {
            var id = Random.Range(1, 3);
            if (id == 1)
            {
                for (int k = 0; k < bulletSpawns.Length; k++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        yield return new WaitForSeconds(0.2f);
                        Instantiate(bullet, bulletSpawns[k].transform.position, Quaternion.identity);
                    }
                }
            }
            if (id == 2)
            {
                for (int k = 0; k < bulletSpawns.Length; k++)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        var g = Instantiate(arrow, bulletSpawns[k].transform.position, Quaternion.identity);
                        Vector3 dir = player.transform.position - g.transform.position;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        g.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
                }
            }
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator mirror()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (player.transform.position.x > transform.position.x)
            {
                //rb.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                //rb.MovePosition(transform.position + -Vector3.right * speed * Time.deltaTime);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void OnDestroy()
    {
        playerCamera.SetActive(true);

        Camera.main.orthographicSize = startOrthSize;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            FindObjectOfType<ShadowTileMap>().size = vector;
        }
        FindObjectOfType<WorldManager>().thirdBoss = true;
    }
}

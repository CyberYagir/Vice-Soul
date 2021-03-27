using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
public class Boss3 : MonoBehaviour
{
    public Vector3 speed;
    public int wormLength;
    [Space]
    public List<Transform> parts;
    public GameObject wormPart;
    public float bonesSpeed;
    public float bonesRot;
    public float lookSpeed;
    public Sprite end, skelPart, skelHead;
    public Tilemap main,decor;
    public List<GameObject> mouth;
    public List<GameObject> oresSpawn;
    public TileBase tile;

    [Space]
    public bool shotPlayer;
    public Vector3 backPos;
    public MobStats mobStats;
    public bool first;
    private Player2D player;
    public GameObject camera, playerCamera;
    private float startOrthSize;
    [Header("UI")]
    public TMP_Text header;
    public GameObject phrase;
    public bool isDead;
    private Vector2Int vector;

    // Update is called once per frame
    private void Start()
    {
        main = FindObjectOfType<WorldGenerator>().main;
        decor = FindObjectOfType<WorldGenerator>().decor;
        for (int i = 0; i < wormLength; i++)
        {
            Transform distance = Instantiate(wormPart, null).transform;
            distance.transform.position = transform.position;
            distance.transform.localScale = transform.localScale;
            distance.gameObject.SetActive(true);
            parts.Add(distance);
        }
        player = FindObjectOfType<Player2D>();
        playerCamera = Camera.main.gameObject;
        startOrthSize = Camera.main.orthographicSize;
    }


    void Update()
    {
        Camera.main.orthographicSize = 30;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            FindObjectOfType<ShadowTileMap>().size = new Vector2Int(58, 35);
        }

        Vector3 intoPlane = Vector3.forward;
        Vector3 toTarget = (player.transform.position + (shotPlayer ? backPos : new Vector3())) - transform.position;
        transform.rotation = Quaternion.LookRotation(intoPlane, -toTarget);
        transform.eulerAngles -= new Vector3(0, 0, 90);



        if (player.GetComponent<PlayerStats>().localPlayer.health <= 0)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector3(transform.position.x, 100, 0), 2 * Time.deltaTime);
            UpdateParts();
            if (transform.position.y > 80)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (player.transform.position.x < transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipY = false;
        }


        if (mobStats.hp <= 0)
        {
            if (parts.Count > 1)
            {
                mobStats.hp = mobStats.maxHp;
                GetComponent<MobDrop>().end = false;
                RemovePart();
            }
            else
            {
                if (first == false)
                {
                    RemovePart();
                    wormLength += 5;
                    speed *= 2;
                    for (int i = 0; i < wormLength; i++)
                    {
                        Transform distance = Instantiate(wormPart, null).transform;
                        distance.transform.position = transform.position;
                        distance.transform.localScale = transform.localScale;
                        distance.GetComponent<SpriteRenderer>().sprite = skelPart;
                        distance.gameObject.SetActive(true);
                        parts.Add(distance);
                    }
                    bonesRot = 0.6f;
                    transform.GetComponent<SpriteRenderer>().sprite = skelHead;
                    first = true;
                    return;
                }
                else
                {
                    if (!isDead)
                    {
                        for (int i = 0; i < mouth.Count; i++)
                        {
                            Destroy(mouth[i].gameObject);
                        }
                        speed = Vector3.zero;
                        playerCamera.SetActive(false);
                        camera.SetActive(true);
                        phrase.SetActive(true);
                        Destroy(gameObject, 45);
                        isDead = true;
                    }
                }
            }
        }
        if (isDead)
        {
            transform.Rotate(Vector3.forward, 5 * Time.deltaTime);
            transform.Translate(Vector3.up * 1 * Time.deltaTime);
        }
        UpdateParts();
    }
    private void FixedUpdate()
    {
        header.text = "Yaksha - " + mobStats.hp + "/" + mobStats.maxHp + " x" + parts.Count;
        for (int i = 0; i < mouth.Count; i++)
        {
            if (mouth[i] != null)
            {
                main.SetTile(main.WorldToCell(mouth[i].transform.position), null);
                decor.SetTile(decor.WorldToCell(mouth[i].transform.position), null);
                decor.SetTile(decor.WorldToCell(mouth[i].transform.position) + new Vector3Int(0,1,0), null);
            }
        }
        for (int i = 0; i < oresSpawn.Count; i++)
        {
            if (main.GetTile(main.WorldToCell(oresSpawn[i].transform.position)) != null)
            {
                main.SetTile(main.WorldToCell(oresSpawn[i].transform.position), tile);
            }
        }
    }

    void UpdateParts()
    {

        transform.Translate((shotPlayer ? speed*4 : speed)  * Time.deltaTime);
        for (int i = 0; i < parts.Count; i++)
        {
            if (i == 0)
            {
                parts[i].transform.rotation = Quaternion.Lerp(parts[i].rotation, transform.GetChild(1).rotation, bonesRot);
                parts[i].transform.position = Vector3.MoveTowards(parts[i].position, transform.GetChild(1).position, (bonesSpeed / i + 1) * Time.deltaTime);

            }
            else
            {
                parts[i].transform.rotation = Quaternion.Lerp(parts[i].rotation, parts[i - 1].rotation, bonesRot);
                parts[i].transform.position = Vector3.MoveTowards(parts[i].position, parts[i - 1].GetChild(0).position, (bonesSpeed / i + 1) * Time.deltaTime);
            }
            if (first == false)
            {
                if (i == parts.Count - 1)
                {
                    parts[i].GetComponent<SpriteRenderer>().sprite = end;
                }
                else
                {
                    parts[i].GetComponent<SpriteRenderer>().sprite = wormPart.GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
    }

    public void RemovePart()
    {
        Destroy(parts[parts.Count - 1].gameObject);
        parts.RemoveAt(parts.Count - 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (shotPlayer == false)
            {
                shotPlayer = true;
                backPos = new Vector3(Random.Range(-100, 100), ( !first ? -100 : Random.Range(-50,50)), Random.Range(-100, 100));
                StopAllCoroutines();
                StartCoroutine(wait());
                if (collision.GetComponent<PlayerStats>() != null) {
                    collision.GetComponent<PlayerStats>().TakeDamage(mobStats.attack);
                    return;
                }
                else
                {
                    collision.transform.parent.GetComponent<PlayerStats>().TakeDamage(mobStats.attack);
                }
            }
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < parts.Count; i++)
        {
            Destroy(parts[i].gameObject);
        }
        playerCamera.SetActive(true);

        Camera.main.orthographicSize = startOrthSize;
        if (FindObjectOfType<ShadowTileMap>() != null)
        {
            FindObjectOfType<ShadowTileMap>().size = vector;
        }
        FindObjectOfType<WorldManager>().thirdBoss = true;
        GameObject aye = Instantiate(FindObjectOfType<WorldManager>().NPCs[4].gameObject);
        aye.GetComponent<NPC>().firstStart = true;
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(4);
        shotPlayer = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NormalWorm : MonoBehaviour
{
    public Vector3 speed;
    public int wormLength;
    [Space]
    public List<Transform> parts;
    public GameObject wormPart;
    public float bonesSpeed;
    public float bonesRot;
    public float lookSpeed;

    [Space]
    public bool shotPlayer;
    public Vector3 backPos;
    public MobStats mobStats;
    private Player2D player;
    [Space]
    public float destroyDist;
    public bool isDead;
    public GameObject deadParticles;

    // Update is called once per frame
    private void Start()
    {
        for (int i = 0; i < wormLength; i++)
        {
            Transform distance = Instantiate(wormPart, null).transform;
            distance.transform.position = transform.position;
            distance.transform.localScale = transform.localScale;
            distance.gameObject.SetActive(true);
            distance.GetComponent<MobInfo>().mobStats = mobStats;
            parts.Add(distance);
        }
        player = FindObjectOfType<Player2D>();
    }


    void Update()
    {
        Vector3 intoPlane = Vector3.forward;
        Vector3 toTarget = (player.transform.position + (shotPlayer ? backPos : new Vector3())) - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(intoPlane, -toTarget), lookSpeed);
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
            GameObject g = Instantiate(deadParticles.gameObject, transform.position, Quaternion.identity);
            g.SetActive(true);
            for (int i = 0; i < parts.Count; i++)
            {
                GameObject g1 = Instantiate(deadParticles.gameObject, parts[i].transform.position, Quaternion.identity);
                g1.SetActive(true);
            }
            Destroy(gameObject, 0.2f);
            return;
        }
        UpdateParts();
    }
    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > destroyDist) {
            Destroy(gameObject);
        }
    }
    void UpdateParts()
    {

        transform.Translate(speed * Time.deltaTime);
        for (int i = 0; i < parts.Count; i++)
        {
            if (i == 0)
            {
                parts[i].transform.rotation = Quaternion.Lerp(parts[i].rotation, transform.GetChild(1).rotation, bonesRot);
                parts[i].transform.position = Vector3.MoveTowards(parts[i].position, transform.GetChild(1).position, (bonesSpeed / i + 1) * (Time.deltaTime+1));

            }
            else
            {
                parts[i].transform.rotation = Quaternion.Lerp(parts[i].rotation, parts[i - 1].rotation, bonesRot);
                parts[i].transform.position = Vector3.MoveTowards(parts[i].position, parts[i - 1].GetChild(0).position, (bonesSpeed / i + 1) * (Time.deltaTime + 1));
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
                backPos = new Vector3(Random.Range(-100, 100), Random.Range(-50,-100), Random.Range(-100, 100));
                StopAllCoroutines();
                StartCoroutine(wait());
                if (collision.GetComponent<PlayerStats>() != null)
                {
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
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(4);
        shotPlayer = false;
    }
}

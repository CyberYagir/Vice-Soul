using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bed : MonoBehaviour
{
    bool triggered;
    public Transform point;
    public bool onBed;
    Player2D player;
    BuildDig buildDig;
    PlayerHead head;
    Entity entity;
    public GameObject canvas, button, menu, content, item;
    private void Start()
    {
        player = FindObjectOfType<Player2D>();
        entity = GetComponent<Entity>();
        buildDig = FindObjectOfType<BuildDig>();
        head = player.GetComponentInChildren<PlayerHead>();
    }
    IEnumerator heal()
    {
        while (true)
        {

            yield return new WaitForSeconds(2);
            player.GetComponent<PlayerStats>().localPlayer.health++;
        }
    }
    private void Update()
    {
        if (onBed)
        {
            player.enabled = false;
            buildDig.enabled = false;
            head.enabled = false;
            player.transform.localEulerAngles = new Vector3(0, 0, 90);
            player.transform.position = new Vector3(point.position.x, point.position.y, player.transform.position.z);
            player.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.cam.eulerAngles = new Vector3(0, 0, 0);
            player.animator.enabled = false;
            if (player.GetComponent<PlayerStats>().localPlayer.damageParticles.active == true)
            {
                player.enabled = true;
                buildDig.enabled = true;
                player.transform.localEulerAngles = new Vector3(0, 0, 0);
                player.cam.localEulerAngles = new Vector3(0, 0, 0);
                head.enabled = true;
                player.animator.enabled = true;
                Time.timeScale = 1;
                StopAllCoroutines();
                onBed = false;
            }
        }
        if (player.GetComponent<PlayerStats>().localPlayer.health <= 0)
        {
            StopAllCoroutines();
        }

        button.SetActive(triggered && onBed == false);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(false);
        }
        if (!triggered)
        {
            menu.SetActive(false);
        }
        if (triggered)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (entity.data != "")
                {
                    if (FindObjectsOfType<NPC>().ToList().Find(x=>x.GetComponent<NPC>().NPCName == entity.data) == null)
                    {
                        entity.data = "";
                    }
                }
                if (entity.data == "")
                {
                    FindObjectOfType<PlayerStart>().playerStart = FindObjectOfType<Player2D>().transform.position;
                    if (!onBed)
                    {
                        StartCoroutine(heal());
                        player.enabled = false;
                        buildDig.enabled = false;
                        player.transform.localEulerAngles = new Vector3(0, 0, 90);
                        player.cam.eulerAngles = new Vector3(0, 0, 0);
                        head.enabled = false;
                        player.animator.enabled = false;
                        Time.timeScale = 2;
                        onBed = true;
                        return;
                    }
                    if (onBed)
                    {
                        StopCoroutine(heal());
                        player.enabled = true;
                        buildDig.enabled = true;
                        player.transform.localEulerAngles = new Vector3(0, 0, 0);
                        player.cam.localEulerAngles = new Vector3(0, 0, 0);
                        head.enabled = true;
                        player.animator.enabled = true;
                        Time.timeScale = 1;
                        onBed = false;
                        return;
                    }
                }
            }
        }
    }
    private void OnDestroy()
    {
        FindObjectOfType<PlayerStart>().playerStart = FindObjectOfType<PlayerStart>().playerStartGlobal;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            triggered = true;
        }
    }
    public void UpdateMenu()
    {
        foreach (Transform item in content.transform)
        {
            Destroy(item.gameObject);
        }
        var npcs = FindObjectsOfType<NPC>().ToList().FindAll(x=>x.GetComponent<MobStats>() == null).ToArray();
        for (int i = 0; i < npcs.Length; i++)
        {
            var it = Instantiate(item.gameObject, content.transform).GetComponent<BedItem>();
            it.image.sprite = npcs[i].icon;
            it.name.rusText = npcs[i].name.rusText;
            it.info.rusText = npcs[i].desc.rusText;

            it.name.engText = npcs[i].name.engText;
            it.info.engText = npcs[i].desc.engText;
            it.npc = npcs[i].gameObject;
            it.gameObject.SetActive(true);
        }

    }

    public void OpenClose(GameObject gameObject)
    {
        gameObject.active = !gameObject.active;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            StopAllCoroutines();
            triggered = false;
        }
    }
}

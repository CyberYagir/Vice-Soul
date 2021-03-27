using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    public Player2D player2D;
    public GameObject canvas;

    public Vector2 playerStart, playerStartGlobal;

    public WorldGenerator worldGenerator;

    public string isStart;

    public GameObject F1;

    public bool ended;

    private void Start()
    {
        canvas.SetActive(true);
        player2D.enabled = false;
        isStart = PlayerPrefs.GetString("Start");
        StartCoroutine(wai1());
    }

    private void Update()
    {
        if (ended == false)
        {
            if (isStart == "False")
            {
                if (worldGenerator.startGen)
                {
                    transform.position = new Vector3((worldGenerator.chunksHorizontalCount * worldGenerator.pixWidth) / 2, worldGenerator.player.transform.position.y, worldGenerator.player.transform.position.z);
                    //StopAllCoroutines();
                    StartCoroutine(wait());
                    ended = true;
                    Instantiate(FindObjectOfType<WorldManager>().NPCs[0]);
                    F1.SetActive(true);
                }
            }
            else
            {
                canvas.GetComponent<Animator>().Play("Start");
                player2D.enabled = true;
            }
        }
    }
    //GENERATION ERROR
    IEnumerator wai1()
    {
        int seconds = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);
            seconds++;
            if (player2D.startCollide)
            {
                yield break;
            }
            if (seconds > 40)
            {
                File.Delete(FindObjectOfType<WorldGenerator>().path);
                GetComponent<PlayerUI>().errorCanvas.SetActive(true);
                yield break;
            }
        }
    }
    IEnumerator wait()
    {
        bool OnGround = false;
        while (OnGround == false)
        {
            yield return new WaitForSeconds(1.5f);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + 2, transform.position.y + 20), -Vector2.up);
            if (hit.collider != null)
            {
                if (hit.transform.gameObject.layer == 0)
                {
                    transform.position = new Vector3(hit.point.x, hit.point.y, 0) + new Vector3(0, 4, transform.position.z);
                    playerStart = new Vector3(hit.point.x, hit.point.y, 0) + new Vector3(0, 4, transform.position.z);
                    playerStartGlobal = new Vector3(hit.point.x, hit.point.y, 0) + new Vector3(0, 4, transform.position.z);
                }
            }
            hit = Physics2D.Raycast(new Vector2(transform.position.x + 2, transform.position.y + 20), -Vector2.up);
            if (hit.collider != null)
            {
                OnGround = true;
            }
        }
        rigidbody2D.velocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);
        canvas.GetComponent<Animator>().Play("Start");
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(FindObjectOfType<WorldGenerator>().Save());
        player2D.enabled = true;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public GameObject rig;
    public Rigidbody2D rb;
    public PlayerStats stats;
    public PlayerStart start;
    public float time;
    public bool isDead;
    public GameObject canvas;
    public GameObject deathMark;

    private void Update()
    {
        if (isDead == false)
        {
            if (stats.localPlayer.health <= 0)
            {
                Instantiate(GetComponent<PlayerStats>().localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                Instantiate(deathMark, Vector3Int.CeilToInt(transform.position)- new Vector3(1.5f, 1f), Quaternion.identity);
                rig.SetActive(false);
                GetComponent<BuildDig>().enabled = false;
                GetComponent<BuildDig>().hands.sprite = null;
                rb.bodyType = RigidbodyType2D.Static;
                rb.velocity = Vector3.zero;
                canvas.SetActive(true);
                StartCoroutine(wait());
                isDead = true;
            }
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(time);
        canvas.SetActive(false);
        rig.SetActive(true);
        GetComponent<BuildDig>().enabled = true;
        stats.localPlayer.health = stats.localPlayer.stdHealth + stats.localPlayer.dopHealth;
        transform.position = start.playerStart;
        rb.bodyType = RigidbodyType2D.Dynamic;
        FindObjectOfType<Sun>().light.transform.position = transform.position;
        isDead = false;
    }
}

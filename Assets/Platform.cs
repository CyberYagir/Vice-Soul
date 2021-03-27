using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Rigidbody2D player;
    public Collider2D collider;
    private void Start()
    {
        player = FindObjectOfType<Player2D>().rb;
    }

    private void Update()
    {
        if ((player.transform.position.y - player.transform.localScale.y/2)>= transform.position.y+ 0.9f)
        {
            collider.enabled = true;
        }
        if ((player.transform.position.y + player.transform.localScale.y / 2) <= transform.position.y + 0.9f)
        {
            collider.enabled = false;
            return;
        }
        if (Input.GetKey(KeyCode.S))
        {
            collider.enabled = false;
        }
    }
}

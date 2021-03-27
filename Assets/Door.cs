using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool triggered;

    public GameObject open, close;

    public bool state;
    public Player2D player;
    public BoxCollider2D boxCollider;


    private void Start()
    {
        player = FindObjectOfType<Player2D>();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var dist = Vector2.Distance(player.transform.position, transform.position + new Vector3(0.5f, 1.5f));
            if (dist < 2f && dist > 0.5f)
            {
                state = !state;
                if (state)
                {
                    if (player.transform.localScale.x < 0)
                    {
                        open.transform.localScale = new Vector3(Mathf.Abs(open.transform.localScale.x), open.transform.localScale.y, 1);
                    }
                    if (player.transform.localScale.x > 0)
                    {
                        open.transform.localScale = new Vector3(-Mathf.Abs(open.transform.localScale.x), open.transform.localScale.y, 1);
                    }

                    open.SetActive(true);
                    close.SetActive(false);
                    boxCollider.enabled = false;
                }
                else
                {
                    open.SetActive(false);
                    close.SetActive(true);
                    boxCollider.enabled = true;
                }
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            triggered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            triggered = false;
        }
    }
}

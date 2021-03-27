using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Spawn : MonoBehaviour
{
    bool triggered;
    public GameObject worm;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (FindObjectOfType<WorldManager>().secondBoss)
            {
                if (FindObjectOfType<PlayerUI>().FindItemInInventoryByName("_EmWorm_"))
                {
                    FindObjectOfType<PlayerUI>().RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName("_EmWorm_"));
                    Instantiate(worm);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
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

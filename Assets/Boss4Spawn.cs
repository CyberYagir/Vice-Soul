using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4Spawn : MonoBehaviour
{
    bool triggered;
    public GameObject snower;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (FindObjectOfType<WorldManager>().thirdBoss)
            {
                if (FindObjectOfType<PlayerUI>().FindItemInInventoryByName("_Snower_"))
                {
                    FindObjectOfType<PlayerUI>().RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName("_Snower_"));
                    Instantiate(snower);
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

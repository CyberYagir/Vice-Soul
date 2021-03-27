using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRandomSentences : MonoBehaviour
{
    public GameObject[] dialogs;
    public GameObject holder;
    public bool triggered;

    private void Start()
    {
        for (int i = 0; i < dialogs.Length; i++)
        {
            dialogs[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (triggered)
        {
            if (transform.localScale.x < 0) {
                holder.transform.localScale = new Vector3(-1,1,1);
            }
            else
            {
                holder.transform.localScale = new Vector3(1, 1, 1);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                GetComponent<NPC>().enabled = false;
                for (int i = 0; i < dialogs.Length; i++)
                {
                    dialogs[i].SetActive(false);
                }
                dialogs[Random.Range(0,dialogs.Length)].SetActive(true);
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
            GetComponent<NPC>().enabled = true;
            for (int i = 0; i < dialogs.Length; i++)
            {
                dialogs[i].SetActive(false);
            }
            triggered = false;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidFirstDialog : MonoBehaviour
{
    public bool used;
    public NPCRandomSentences NRS;
    public GameObject sameDialog;
    private bool triggered;

    private void Update()
    {
        if (used == false)
        {
            if (triggered)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    sameDialog.SetActive(true);
                    GetComponent<NPC>().enabled = false;
                    NRS.enabled = false;
                    FindObjectOfType<Player2D>().enabled = false;
                    FindObjectOfType<BuildDig>().enabled = false;
                }
            }
        }
    }

    public void Yes()
    {
        used = true;
        NRS.enabled = true;
        GetComponent<NPC>().enabled = true;
        FindObjectOfType<Player2D>().enabled = true;
        FindObjectOfType<BuildDig>().enabled = true;
        FindObjectOfType<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName("_StoneSw_"));
        sameDialog.SetActive(false);
    }
    public void No()
    {
        sameDialog.SetActive(false);
        NRS.enabled = true;
        GetComponent<NPC>().enabled = true;
        FindObjectOfType<Player2D>().enabled = true;
        FindObjectOfType<BuildDig>().enabled = true;
        GetComponent<Animator>().Play("GuideFade");
        used = true;
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            sameDialog.SetActive(false);
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            FindObjectOfType<Player2D>().enabled = true;
            FindObjectOfType<BuildDig>().enabled = true;
            sameDialog.SetActive(false);
            triggered = false;
        }
    }
}

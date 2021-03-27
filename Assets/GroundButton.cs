using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundButton : MonoBehaviour
{
    public bool on;


    private void Update()
    {
        if (on)
        {
            RaycastHit2D down = Physics2D.Raycast(transform.position + new Vector3(0,-2, -5) + new Vector3(0.5f, 0.5f), Vector3.forward);
            if (down.collider != null)
            {
                if (down.collider.gameObject.GetComponent<EnergyActive>() != null)
                down.collider.gameObject.GetComponent<EnergyActive>().active = (down.collider.gameObject.GetComponent<EnergyActive>() != null);
            }
            RaycastHit2D left = Physics2D.Raycast(transform.position + new Vector3(-1, 0, -5) + new Vector3(0.5f,0.5f), Vector3.forward);
            Debug.DrawRay(transform.position + new Vector3(-1, 0, -5), Vector3.forward);
            if (left.collider != null)
            {
                if (left.collider.gameObject.GetComponent<EnergyActive>() != null)
                    left.collider.gameObject.GetComponent<EnergyActive>().active = (left.collider.gameObject.GetComponent<EnergyActive>() != null);
            }
            RaycastHit2D right = Physics2D.Raycast(transform.position + new Vector3(1, 0, -5) + new Vector3(0.5f, 0.5f), Vector3.forward);
            if (right.collider != null)
            {
                if (right.collider.gameObject.GetComponent<EnergyActive>() != null)
                    right.collider.gameObject.GetComponent<EnergyActive>().active = (right.collider.gameObject.GetComponent<EnergyActive>() != null);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() != null)
        {
            on = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() != null)
        {
            on = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        on = false;
    }

}

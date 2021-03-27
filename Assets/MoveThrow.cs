using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveThrow : MonoBehaviour
{
    public Transform p1,p2;
    public CapsuleCollider2D capsuleCollider;
    public CapsuleCollider2D player;
    private void Start()
    {
        player = FindObjectOfType<PlayerStats>().GetComponent<Hook>().capsule;
    }
    public void FixedUpdate()
    {
        Physics2D.IgnoreCollision(capsuleCollider, player, false);
    }
    private void OnTriggerEnter(Collider other)
    {
        print("stay");
        if (Input.GetKey(KeyCode.A))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0, 10), Vector3.forward);
            bool can = false;
            if (hit.collider == null)
            {

                can = true;
            }
            if (hit.collider != null)
            {
                if (hit.transform.gameObject.layer != 0)
                    can = true;
            }
            if (can)
            other.transform.position = new Vector3(p1.position.x, other.transform.position.y, other.transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0, 10), Vector3.forward);
            bool can = false;
            if (hit.collider == null)
            {

                can = true;
            }
            if (hit.collider != null)
            {
                if (hit.transform.gameObject.layer != 0)
                    can = true;
            }
            if (can) { 
                other.transform.position = new Vector3(p2.position.x, other.transform.position.y, other.transform.position.z);
            }
        }
    }
}

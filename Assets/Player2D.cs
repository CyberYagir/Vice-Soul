using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[System.Serializable]
public class Player2D : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public Rigidbody2D rb;
    public bool jump;

    public float jumpFeetsDist, feets;

    public SpriteRenderer sprite;

    public Animator animator;
    public LayerMask mask = -1;
    Vector3 stdSize;
    public float xOffcet;
    public Transform cam;

    public bool startCollide;

    public float oldVel;
    private void Start()
    {
        oldVel = rb.velocity.y;
        stdSize = transform.localScale;
        Physics2D.IgnoreLayerCollision(9, 8);
        Physics2D.IgnoreLayerCollision(10, 8);
        Physics2D.IgnoreLayerCollision(9, 2);
    }

    public void Jump()
    {
        rb.AddRelativeForce(Vector3.up * jumpForce * Time.deltaTime * 30, ForceMode2D.Force);
        jump = true;
    }

    private void Update()
    {

        if (oldVel < -25 && rb.velocity.y == 0)
        {
            GetComponent<PlayerStats>().localPlayer.health -= Math.Abs((int)oldVel);
        }
        oldVel = rb.velocity.y;

    

        if (jump == true)
        {
            animator.Play("Jump");
        }
        else
        {
            if (rb.velocity.normalized.x != 0)
            {
                animator.Play("Walk");
            }
            else
            {
                animator.Play("Idle");
            }
        }


        rb.AddRelativeForce(Vector3.right * speed * Time.deltaTime * Input.GetAxisRaw("Horizontal"), ForceMode2D.Force);

        if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
        {
            transform.localScale = new Vector3(stdSize.x, stdSize.y, stdSize.z);
        }
        
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {

            transform.localScale = new Vector3(-stdSize.x, stdSize.y, stdSize.z);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (jump == false)
            {
                rb.AddRelativeForce(Vector3.up * jumpForce * Time.deltaTime * 10, ForceMode2D.Force);
                jump = true;
            }
        }
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - xOffcet, transform.position.y - transform.localScale.y - feets), -Vector2.up, Mathf.Infinity, mask);
        RaycastHit2D hitRight= Physics2D.Raycast(new Vector2(transform.position.x + xOffcet, transform.position.y - transform.localScale.y - feets), -Vector2.up, Mathf.Infinity, mask);
        Debug.DrawRay(new Vector2(transform.position.x - xOffcet, transform.position.y - transform.localScale.y - feets), -Vector3.up);
        Debug.DrawRay(new Vector2(transform.position.x + xOffcet, transform.position.y - transform.localScale.y - feets), -Vector3.up);



        if (hitLeft.collider != null || hitRight.collider != null)
        {
            if ((!hitLeft.collider.isTrigger && hitLeft.transform.tag != "Tree") || (!hitRight.collider.isTrigger && hitRight.transform.tag != "Tree"))
            {
                if (hitLeft.transform.gameObject.layer == 0 || hitRight.transform.gameObject.layer == 0)
                {
                    if (Vector2.Distance(hitLeft.point, new Vector2(transform.position.x, transform.position.y - transform.localScale.y - feets)) <= jumpFeetsDist ||
                        Vector2.Distance(hitRight.point, new Vector2(transform.position.x, transform.position.y - transform.localScale.y - feets)) <= jumpFeetsDist)
                    {
                        jump = false;
                    }
                    else
                    {
                        jump = true;
                    }
                }
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (startCollide == false)
        {
            startCollide = true;
            if (jump == false)
            {
                transform.position = new Vector2(Mathf.RoundToInt(transform.position.x) + 0.5f, transform.position.y);
            }
        }
    }
    
}

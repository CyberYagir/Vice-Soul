using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{

    public GameObject hook;
    public float hookSpeed;
    public float hookDist;
    public LineRenderer lineRenderer;
    public bool hooked, max = false, triggered, playerposSeted;
    public Vector3 dir, pos;
    public Vector3 playerSetPos;
    public LayerMask layerMask;
    public CapsuleCollider2D capsule;
    private void Start()
    {
        hook.transform.parent = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!triggered)
            {
                pos = Vector3.zero;
                hook.transform.localPosition = transform.position;
                dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;


                Vector3 intoPlane = Vector3.forward;
                hook.transform.rotation = Quaternion.LookRotation(intoPlane, -dir);
                hook.transform.eulerAngles -= new Vector3(0, 0, 90);


                hooked = true;
                return;
            }
            else
            {
                GetComponent<Player2D>().enabled = true;
                //GetComponent<Player2D>().Jump();
                capsule.enabled = true;
                GetComponent<Rigidbody2D>().gravityScale = 5;
                max = false;
                hook.SetActive(false);
                hooked = false;
                playerposSeted = false;
                triggered = false;
                lineRenderer.gameObject.SetActive(false);


                pos = Vector3.zero;
                hook.transform.localPosition = transform.position;
                dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                playerposSeted = false;
                Vector3 intoPlane = Vector3.forward;
                hook.transform.rotation = Quaternion.LookRotation(intoPlane, -dir);
                hook.transform.eulerAngles -= new Vector3(0, 0, 90);
                max = false;

                hooked = true;
                triggered = false;
            }
        }
        if (hooked && triggered == false)
        {
            lineRenderer.gameObject.SetActive(true);
            lineRenderer.SetPosition(0, transform.position);

            hook.SetActive(true);
            if (max == false)
            {
                RaycastHit2D hit = Physics2D.Raycast(hook.transform.position - new Vector3(0, 0, -10), Vector3.forward);
                if (hit.collider != null)
                {
                    if (hit.transform.gameObject.layer == 0)
                    {
                        triggered = true;
                        return;
                    }
                }
                pos += dir * hookSpeed * Time.deltaTime;
                if (Vector2.Distance(hook.transform.position, transform.position) > hookDist)
                {
                    max = true;
                }
                hook.transform.position = transform.position + pos;
            }
            else
            if (max == true)
            {
                print("Lerp");
                hook.transform.position = Vector3.Lerp(hook.transform.position, transform.position, hookSpeed * Time.deltaTime);
                if (Vector2.Distance(hook.transform.position, transform.position) <= 0.5f)
                {

                    lineRenderer.gameObject.SetActive(false);
                    max = false;
                    hook.SetActive(false);
                    hooked = false;
                    return;
                }
            }

            Debug.DrawRay(transform.position, dir, Color.green, 5);
            lineRenderer.SetPosition(1, hook.transform.position);
        }


        if (hooked && triggered)
        {
            capsule.enabled = false;
            lineRenderer.SetPosition(0, new Vector2(transform.position.x, transform.position.y) );
            var dir = hook.transform.position - transform.position;
            bool can = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);
            GetComponent<Player2D>().enabled = false;
            if (hit.collider != null)
            {
                print("!=null " + hit.transform.gameObject.layer + " " + hit.transform.name);
                if (hit.transform.gameObject.layer == 0)
                {
                    print("layer == 0");
                    can = true;
                }
            }

            lineRenderer.SetPosition(1, new Vector2(hit.point.x, hit.point.y));
            if (can == true && !playerposSeted)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, hit.point.y, transform.position.z), hookSpeed * Time.deltaTime);
                if (Vector2.Distance((Vector2)transform.position, new Vector2(hit.point.x, hit.point.y)) <= 1f)
                {
                    playerSetPos = transform.position;
                    playerposSeted = true;
                }
            }
            if (can && playerposSeted)
            {
                transform.position = playerSetPos;
            }
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Player2D>().jump = false;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<Player2D>().enabled = true;
                //GetComponent<Player2D>().Jump();
                capsule.enabled = true;
                GetComponent<Rigidbody2D>().gravityScale = 5;
                max = false;
                hook.SetActive(false);
                hooked = false;
                playerposSeted = false;
                triggered = false;
                lineRenderer.gameObject.SetActive(false);
                return;
            }
        }
    }
}

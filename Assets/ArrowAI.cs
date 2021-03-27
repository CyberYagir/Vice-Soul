using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAI : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public Rigidbody2D rb;
    private void Start()
    {
       float maxHP = -999;
       var id = -1;
       var mobs = FindObjectsOfType<MobStats>();
        for (int i = 0; i < mobs.Length; i++)
        {
            if (mobs[i].hp > maxHP)
            {
                id = i;
                maxHP = mobs[i].hp;
            }
        }

        if (id != -1)
        {
            target = mobs[id].gameObject;
        }

        Destroy(gameObject, 5);
    }


    private void Update()
    {
        if (target != null)
        {
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            Vector3 intoPlane = Vector3.forward;
            Vector3 toTarget = target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(intoPlane, -toTarget);
            transform.eulerAngles -= new Vector3(0, 0, 90);
            transform.Translate(Vector2.right * speed * Time.deltaTime);

            }
        }
    }
}

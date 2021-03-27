using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4Bullet : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5);   
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}

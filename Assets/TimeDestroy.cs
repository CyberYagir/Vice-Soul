using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDestroy : MonoBehaviour
{
    public float time = 10;


    private void Start()
    {
        Destroy(gameObject, time);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBomb : MonoBehaviour
{
    public float time;

    private void Start()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(time);
        GetComponent<TNT>().Boom();
    }
}

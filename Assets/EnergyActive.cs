using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyActive : MonoBehaviour
{
    public bool active;

    private void Start()
    {
        StartCoroutine(loop());
    }

    IEnumerator loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            active = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidDamage : MonoBehaviour
{
    public int effect;
    public Light light;

    private void Start()
    {
        if (light != null)
        {
            light.gameObject.SetActive(Random.Range(0, 2) == 1 ? true : false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.tag == "Player")
        {
            FindObjectOfType<Player2D>().GetComponent<Effects>().AddEffect(effect);
        }
    }
}

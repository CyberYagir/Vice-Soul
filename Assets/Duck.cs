using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    public GameObject deadParticles;
    public MobStats stats;
    public bool dead;



    private void Update()
    {
        if (dead == false)
        {
            if (stats.hp <= 0)
            {
                GameObject g = Instantiate(deadParticles.gameObject, transform.position, Quaternion.identity);
                g.SetActive(true);
                dead = true;
                Destroy(gameObject, 0.2f);
            }
        }
    }
}

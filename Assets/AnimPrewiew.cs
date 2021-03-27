using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPrewiew : MonoBehaviour
{
    public Zombie zombie;
    public Spider spider;

    public bool run;
    public bool idle;
    public bool attack;


    private void Update()
    {
        if (zombie != null)
        {
            if (run)
                zombie.GetComponent<Animator>().Play(zombie.Walk);

            if (idle)
                zombie.GetComponent<Animator>().Play(zombie.Idle);

            if (attack)
                zombie.GetComponent<Animator>().Play(zombie.attack);
        }
        if (spider != null)
        {
            if (run)
                spider.GetComponent<Animator>().Play(spider.walk);

            if (idle)
                spider.GetComponent<Animator>().Play(spider.idle);

            if (attack)
                spider.GetComponent<Animator>().Play(spider.attack);
        }
    }
}

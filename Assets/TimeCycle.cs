using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCycle : MonoBehaviour
{
    public float timeLine;
    public int endTimeLine;
    public float hours, minutes;
    public TMPro.TMP_Text text;
    public bool isNight;
    IEnumerator cy;

    public void Start()
    {
        StartCoroutine(cycle());
    }

    private void Update()
    {
        text.text = System.String.Format("{0:00}", (int)hours) + ":" + System.String.Format("{0:00}", (int)minutes) ;
        if (hours > 22)
            if (cy == null)
            {
                StopAllCoroutines();
                cy = cycle();
                StartCoroutine(cy);
            }
        if (hours < 5 || hours > 20)
        {
            isNight = true;
        }
        if (hours >= 5 && hours <= 20)
        {
            isNight = false;
        }
    }
        IEnumerator cycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                timeLine += 0.5f;
                if (timeLine >= endTimeLine)
                {
                    timeLine = 0;
                }
                hours = (float)(timeLine) / 25;
                minutes = (Mathf.Abs(Mathf.FloorToInt(hours) - hours) * 10) * 6;
                hours = (int)hours;
            }

        }

    }

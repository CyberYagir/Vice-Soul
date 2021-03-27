using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alt : MonoBehaviour
{
    public PlayerUI playerUI;
    public Image image;



    private void Update()
    {
        if (playerUI.selectedCase == 9)
        {
            image.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            image.color = playerUI.selectedColor;
        }
        else
        {
            image.transform.localScale = new Vector3(1f, 1f, 1f);
            image.color = playerUI.normalColor;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWorldItem : MonoBehaviour
{
    public bool select;
    public Image image;
    public void Click() {
        var p = FindObjectsOfType<AddWorldItem>();
        for (int i = 0; i < p.Length; i++)
        {
            p[i].select = false;
        }
        select = true;
    }


    private void Update()
    {
        if (select)
        {
            image.color = Color.gray;
        }
        else
        {
            image.color = Color.white;
        }
    }

}

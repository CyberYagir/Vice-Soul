using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldItem : MonoBehaviour
{
    public TMP_Text name, desc, info, size;
    public int index;

    public void Click()
    {
        foreach (Transform item in FindObjectOfType<Menu>().worldHolder.transform)
        {
            item.GetComponent<Image>().color = Color.white;
        }
        FindObjectOfType<Menu>().selectedIndex = index;
        this.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1);
        
    }
}

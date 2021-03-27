using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotBarMouseClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool  over = false;
    public int id;
    public void Update()
    {
        if (over)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                FindObjectOfType<PlayerUI>().selectedCase = id;
                FindObjectOfType<PlayerUI>().UpdateHotbar();
                FindObjectOfType<PlayerUI>().UpdateHotBarBack();
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }
}

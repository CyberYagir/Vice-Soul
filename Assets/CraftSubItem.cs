using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSubItem : MonoBehaviour, IPointerDownHandler
{
    public string ID;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (ID != "")
        {
            FindObjectOfType<PlayerUI>().infoWindow.SetActive(true);
            FindObjectOfType<PlayerUI>().setCraftInfo(ID);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ChestUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject items;
    public GameObject item;
    public bool over;
    public void OnPointerDown(PointerEventData eventData)
    {
        var ui = FindObjectOfType<PlayerUI>();
        if (ui.mouseItem.secondName != "_None_")
        {
            ui.selectedChest.GetComponent<Chest>().AddItemChest(this, Item.Clone(ui.mouseItem));
            ui.mouseItem = new Item();
            ui.selectedChest.GetComponent<Chest>().UpdateChest(this);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FindCraft : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool over;
    public PlayerUI playerUI;
    public WorldManager manager;
    public Image image;
    public string item;
    public string oldItem;
    private void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        manager = FindObjectOfType<WorldManager>();
        oldItem = item;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }




    private void Update()
    {
        if (oldItem != item)
        {
            playerUI.CraftGuideUpdate(item);
            oldItem = item;
        }
        if (item != "")
        {
            image.gameObject.SetActive(true);
            image.sprite = manager.GetItemBySecondName(item).icon;
        }
        else
        {
            image.gameObject.SetActive(false);
            image.sprite = null;
        }
        if (over)
        {
            //PlayerUI inventory.active = true ...
        }
    }
}

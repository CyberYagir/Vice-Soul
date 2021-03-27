using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectArrows : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool over;
    public PlayerUI playerUI;
    public WorldManager manager;
    public Image image;
    private void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        manager = FindObjectOfType<WorldManager>();
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
        if (playerUI.buildDig.selectedArrows != "")
        {
            image.gameObject.SetActive(true);
            image.sprite = manager.GetItemBySecondName(playerUI.buildDig.selectedArrows).icon;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int craftID;
    public Image image;
    public PlayerUI ui;
    public PlayerStats stats;
    public bool over;

    public float time = 0;
    public float speed = 1;

    public bool keyDown;


    private void Start()
    {
        stats = FindObjectOfType<PlayerStats>();
        ui = stats.GetComponent<PlayerUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
        var ui = FindObjectOfType<PlayerUI>();
        var p = FindObjectOfType<WorldManager>().GetItemBySecondName(FindObjectOfType<WorldManager>().crafts[craftID].finalItem);
        ui.SetProperties(p);
        ui.hint.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
        var ui = FindObjectOfType<PlayerUI>();
        ui.hint.SetActive(false);
    }

    public void Click()
    {
        WorldManager world = FindObjectOfType<WorldManager>();

        var manager = FindObjectOfType<WorldManager>();
        for (int p = 0; p < stats.localPlayer.craftingTables.Count; p++)
        {
            if (manager.crafts[craftID].craftType == stats.localPlayer.craftingTables[p])
            {
                int items = 0;
                for (int j = 0; j < manager.crafts[craftID].crafts.Count; j++)
                {
                    for (int k = 0; k < stats.localPlayer.inventory.Count; k++)
                    {
                        if (stats.localPlayer.inventory[k].secondName == manager.crafts[craftID].crafts[j].item)
                        {
                            if (stats.localPlayer.inventory[k].value >= manager.crafts[craftID].crafts[j].value)
                            {
                                items++;
                            }
                        }
                    }
                }
                if (items != manager.crafts[craftID].crafts.Count)
                {
                    GetComponent<Button>().enabled = false;
                    image.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                    break;
                }
            }
        }




        for (int i = 0; i < world.crafts[craftID].crafts.Count; i++)
        {
            var it = world.GetItemBySecondName(world.crafts[craftID].crafts[i].item);
            it.value = world.crafts[craftID].crafts[i].value;
            stats.GetComponent<PlayerUI>().RemoveItem(it, false);
        }

        var craftItems = FindObjectsOfType<CraftItem>();
        for (int i = 0; i < craftItems.Length; i++)
        {
            for (int p = 0; p < stats.localPlayer.craftingTables.Count; p++)
            {
                if (manager.crafts[craftItems[i].craftID].craftType == stats.localPlayer.craftingTables[p])
                {
                    int items = 0;
                    for (int j = 0; j < manager.crafts[craftItems[i].craftID].crafts.Count; j++)
                    {
                        for (int k = 0; k < stats.localPlayer.inventory.Count; k++)
                        {
                            if (stats.localPlayer.inventory[k].secondName == manager.crafts[craftItems[i].craftID].crafts[j].item)
                            {
                                if (stats.localPlayer.inventory[k].value >= manager.crafts[craftItems[i].craftID].crafts[j].value)
                                {
                                    items++;
                                }
                            }
                        }
                    }
                    if (items != manager.crafts[craftItems[i].craftID].crafts.Count)
                    {
                        craftItems[i].GetComponent<Button>().enabled = false;
                        craftItems[i].image.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                        break;
                    }
                }
            }
        }


        var fit = world.GetItemBySecondName(world.crafts[craftID].finalItem);
        fit.value = world.crafts[craftID].finalItemValue;
        stats.GetComponent<PlayerUI>().AddItem(fit, false);
        stats.GetComponent<PlayerUI>().UpdateInventory(false);
    }

    private void Update()
    {
        if (over)
        {
            keyDown = Input.GetKey(KeyCode.Mouse1);
        }
        else
        {
            keyDown = false;
        }
        if (keyDown)
        {
            time += (float)speed * (float)Time.unscaledDeltaTime;
            if(time >= 0.2f)
            {
                ClickMouse();
                time = 0;
                speed += speed * (Time.unscaledDeltaTime*2);
            }
        }
        else
        {
            time = 0.2f;
            speed = 1;
        }
    }

    public void ClickMouse()
    {
        WorldManager world = FindObjectOfType<WorldManager>();
        var manager = FindObjectOfType<WorldManager>();
        if (ui.mouseItem.secondName == "_None_" || ui.mouseItem.secondName == manager.crafts[craftID].finalItem)
        {
            if (ui.mouseItem.value + manager.crafts[craftID].finalItemValue <= ui.mouseItem.maxValue)
            {
                if (GetComponent<Button>().enabled == true)
                {
                    for (int p = 0; p < stats.localPlayer.craftingTables.Count; p++)
                    {
                        if (manager.crafts[craftID].craftType == stats.localPlayer.craftingTables[p])
                        {
                            int items = 0;
                            for (int j = 0; j < manager.crafts[craftID].crafts.Count; j++)
                            {
                                for (int k = 0; k < stats.localPlayer.inventory.Count; k++)
                                {
                                    if (stats.localPlayer.inventory[k].secondName == manager.crafts[craftID].crafts[j].item)
                                    {
                                        if (stats.localPlayer.inventory[k].value >= manager.crafts[craftID].crafts[j].value)
                                        {
                                            items++;
                                        }
                                    }
                                }
                            }
                            if (items != manager.crafts[craftID].crafts.Count)
                            {
                                GetComponent<Button>().enabled = false;
                                image.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < world.crafts[craftID].crafts.Count; i++)
                    {
                        var it = world.GetItemBySecondName(world.crafts[craftID].crafts[i].item);
                        it.value = world.crafts[craftID].crafts[i].value;
                        stats.GetComponent<PlayerUI>().RemoveItem(it, false);
                    }

                    #region craftVisual
                    var craftItems = FindObjectsOfType<CraftItem>();
                    for (int i = 0; i < craftItems.Length; i++)
                    {
                        for (int p = 0; p < stats.localPlayer.craftingTables.Count; p++)
                        {
                            if (manager.crafts[craftItems[i].craftID].craftType == stats.localPlayer.craftingTables[p])
                            {
                                int items = 0;
                                for (int j = 0; j < manager.crafts[craftItems[i].craftID].crafts.Count; j++)
                                {
                                    for (int k = 0; k < stats.localPlayer.inventory.Count; k++)
                                    {
                                        if (stats.localPlayer.inventory[k].secondName == manager.crafts[craftItems[i].craftID].crafts[j].item)
                                        {
                                            if (stats.localPlayer.inventory[k].value >= manager.crafts[craftItems[i].craftID].crafts[j].value)
                                            {
                                                items++;
                                            }
                                        }
                                    }
                                }
                                if (items != manager.crafts[craftItems[i].craftID].crafts.Count)
                                {
                                    craftItems[i].GetComponent<Button>().enabled = false;
                                    craftItems[i].image.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    if (ui.mouseItem.secondName == "_None_")
                    {
                        var fit = world.GetItemBySecondName(world.crafts[craftID].finalItem);
                        fit.value = world.crafts[craftID].finalItemValue;
                        ui.mouseItem = fit;
                        stats.GetComponent<PlayerUI>().UpdateInventory(false);
                    }
                    else
                    if (ui.mouseItem.secondName == manager.crafts[craftID].finalItem)
                    {
                        var fit = world.GetItemBySecondName(world.crafts[craftID].finalItem);
                        fit.value = world.crafts[craftID].finalItemValue;
                        ui.mouseItem.value += world.crafts[craftID].finalItemValue;
                        stats.GetComponent<PlayerUI>().UpdateInventory(false);
                    }
                }
            }
        }
    }
}

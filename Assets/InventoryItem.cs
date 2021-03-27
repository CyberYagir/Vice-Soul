using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int inventoryIndex = 0;
    public Image image;
    public PlayerStats stats;
    public TMP_Text text;

    public bool mouseOver;

    public float stdSpeed, speed;

    public bool keydown;
    public bool canMouseOver = true;
    PlayerUI ui;

    private void Start()
    {
        ui = FindObjectOfType<PlayerUI>();
    }

    public void Init()
    {
        canMouseOver = true;
        image.sprite = stats.localPlayer.inventory[inventoryIndex].icon;
        if (stats.localPlayer.inventory[inventoryIndex].value > 1)
        {
            text.text = stats.localPlayer.inventory[inventoryIndex].value.ToString();
        }
        else
        {
            text.text = "";
        }
    }
    void Awake()
    {
        mouseOver = false;
    }
    public void Click()
    {
        if (canMouseOver)
        {
            ui = stats.localPlayer.controller.GetComponent<PlayerUI>();
            if (stats.localPlayer.inventory[inventoryIndex].inInventory == -1)
            {
                for (int i = 0; i < stats.localPlayer.inventory.Count; i++)
                {
                    if (stats.localPlayer.inventory[i].inInventory == ui.selectedCase)
                    {
                        stats.localPlayer.inventory[i].inInventory = -1;
                    }
                }
                stats.localPlayer.inventory[inventoryIndex].inInventory = ui.selectedCase;
            }
            else
            {
                stats.localPlayer.inventory[inventoryIndex].inInventory = -1;
            }
            ui.UpdateHotbar();
        }
    }

    private void Update()
    {
        if (mouseOver && canMouseOver)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
            {
                keydown = true;
                if (ui.mouseItem.secondName != stats.localPlayer.inventory[inventoryIndex].secondName)
                {
                    bool update = false;
                    if (ui.mouseItem.secondName != "_None_")
                    {
                        update = true;
                        ui.AddItem(ui.mouseItem);
                        ui.mouseItem = new Item();
                    }
                    var it = Item.Clone(stats.localPlayer.inventory[inventoryIndex]);
                    stats.localPlayer.inventory[inventoryIndex].value -= 1;
                    it.value = 1;
                    ui.mouseItem = it;

                    text.text = (stats.localPlayer.inventory[inventoryIndex].value).ToString();
                    ui.hint.SetActive(false);
                    if (update)
                    {
                        ui.UpdateInventory(true);
                        ui.UpdateHotbar();
                    }
                    if (stats.localPlayer.inventory[inventoryIndex].value == 0)
                    {
                        StopAllCoroutines();
                        ui.UpdateInventory(true);
                        ui.UpdateHotbar();
                        keydown = false;
                        gameObject.SetActive(false);
                        speed = 0;
                        canMouseOver = false;
                        return;
                    }
                }
                StartCoroutine(add());
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (ui.mouseItem.secondName != "_None_")
                {
                    ui.AddItem(ui.mouseItem);
                }
                var it = Item.Clone(stats.localPlayer.inventory[inventoryIndex]);
                it.value = stats.localPlayer.inventory[inventoryIndex].value;
                stats.localPlayer.inventory[inventoryIndex].value = 0;
                ui.mouseItem = it;
                ui.hint.SetActive(false);
                ui.UpdateInventory(true);
                ui.UpdateHotbar();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftControl))
            {
                if (ui.selectedChest != null)
                {
                    var it = Item.Clone(stats.localPlayer.inventory[inventoryIndex]);
                    it.value = stats.localPlayer.inventory[inventoryIndex].value;
                    stats.localPlayer.inventory[inventoryIndex].value = 0;
                    ui.selectedChest.GetComponent<Chest>().AddItemChest(ui.chest5x5UI.GetComponent<ChestUI>(), it);

                    ui.UpdateInventory(true);
                    ui.UpdateHotbar();
                }
            }
            if (stats.localPlayer.inventory.Count < inventoryIndex)
            if (stats.localPlayer.inventory[inventoryIndex] != null)
            {
                if (stats.localPlayer.inventory[inventoryIndex].value > 1)
                {
                    text.text = stats.localPlayer.inventory[inventoryIndex].value.ToString();
                }
                else
                {
                    text.text = "";
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopAllCoroutines();
            keydown = false;
        }
        if (keydown)
        {
            if (stats.localPlayer.inventory[inventoryIndex].value == 0)
            {
                ui.UpdateInventory(true);
                ui.UpdateHotbar();
            }
        }
    }


    IEnumerator add()
    {
        speed = 0;
        while (keydown)
        {
            if (!mouseOver)
                yield break;
            yield return new WaitForSecondsRealtime(stdSpeed - speed);
            if (ui.mouseItem.secondName == stats.localPlayer.inventory[inventoryIndex].secondName)
            {
                if (ui.mouseItem.value < stats.localPlayer.inventory[inventoryIndex].maxValue)
                {
                    if (stats.localPlayer.inventory[inventoryIndex].value > 0)
                    {
                        stats.localPlayer.inventory[inventoryIndex].value -= 1;
                        ui.mouseItem.value++;
                        text.text = (stats.localPlayer.inventory[inventoryIndex].value).ToString();
                        if (stats.localPlayer.inventory[inventoryIndex].value == 0)
                        {
                            ui.UpdateInventory(true);
                            keydown = false;
                            speed = 0;
                            StopAllCoroutines();
                            canMouseOver = false;
                            yield break;
                        }
                    }
                    speed += Time.unscaledDeltaTime;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canMouseOver)
        {
            var p = stats.localPlayer.inventory[inventoryIndex];
            ui.SetProperties(p);
            ui.hint.SetActive(true);
        }
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canMouseOver)
        {
            ui.hint.SetActive(false);
        }
        StopAllCoroutines();
        mouseOver = false;
    }
}
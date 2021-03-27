using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class ChestItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int inChestID;
    public Image image;
    public TMP_Text text;

    public bool mouseOver;

    public bool keydown;

    public float stdSpeed, speed;

    Chest chest;

    PlayerUI ui;

    private void Start()
    {
        ui = FindObjectOfType<PlayerUI>();
    }

    private void Update()
    {
        if (mouseOver)
        {
            var chest = ui.selectedChest.GetComponent<Chest>();
            if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (ui.inventoryWindow.active == true)
                {
                    keydown = true;

                    if (ui.mouseItem.secondName != chest.itemsIn[inChestID].secondName)
                    {
                        bool update = false;
                        if (ui.mouseItem.secondName != "_None_")
                        {
                            update = true;
                            ui.AddItem(ui.mouseItem);
                        }
                        chest.itemsIn[inChestID].value -= 1;
                        var it = Item.Clone(chest.itemsIn[inChestID]);
                        it.value = 1;
                        ui.mouseItem = it;
                        if (update)
                        {
                            if (chest.type == Chest.ChestType.x5)
                            {
                                chest.UpdateChest(ui.chest5x5UI.GetComponent<ChestUI>());
                            }
                        }
                    }
                    StartCoroutine(add());
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (ui.mouseItem.secondName != "_None_")
                {
                    ui.AddItem(ui.mouseItem);
                }
                var it = Item.Clone(chest.itemsIn[inChestID]);
                it.value = chest.itemsIn[inChestID].value;
                chest.itemsIn[inChestID].value = 0;
                ui.mouseItem = it;
                ui.UpdateInventory(true); 
                chest.UpdateChest(ui.chest5x5UI.GetComponent<ChestUI>());
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftControl))
            {
                if (ui.selectedChest != null)
                {
                    var it = Item.Clone(chest.itemsIn[inChestID]);
                    it.value = chest.itemsIn[inChestID].value;
                    chest.itemsIn[inChestID].value = 0;
                    ui.AddItem(it, true);
                    chest.UpdateChest(ui.chest5x5UI.GetComponent<ChestUI>());
                    ui.UpdateHotbar();
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
            var chest = ui.selectedChest.GetComponent<Chest>();
            if (chest.itemsIn[inChestID].value <= 0)
            {
                chest.UpdateChest(ui.chest5x5UI.GetComponent<ChestUI>());
            }
        }
    }

    IEnumerator add()
    {
        speed = 0;
        while (keydown)
        {
            if (ui.inventoryWindow.active == true)
            {
                var chest = ui.selectedChest.GetComponent<Chest>();
                yield return new WaitForSecondsRealtime(stdSpeed - speed);
                if (ui.mouseItem.value < chest.itemsIn[inChestID].maxValue)
                {
                    if (chest.itemsIn[inChestID].value > 0)
                    {
                        text.text = chest.itemsIn[inChestID].value.ToString();
                        chest.itemsIn[inChestID].value -= 1;
                        ui.mouseItem.value++;
                        if (chest.itemsIn[inChestID].value == 0)
                        {
                            Destroy(gameObject);
                            keydown = false;
                            yield break;
                        }
                    }
                    speed += Time.unscaledDeltaTime;
                }
            }
            else
            {
                keydown = false;
                break;
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

}

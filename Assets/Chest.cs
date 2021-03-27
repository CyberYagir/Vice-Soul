using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum ChestType { x5 };
    public List<Item> itemsIn = new List<Item>();
    public ChestType type;
    public bool triggered;
    public PlayerUI ui;

    private void Start()
    {
        ui = FindObjectOfType<PlayerUI>();
    }
    private void Update()
    {
        if (triggered)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (ui.chest5x5UI.active == false)
                {
                    ui.selectedChest = gameObject;
                    if (type == ChestType.x5)
                    {
                        ui.chest5x5UI.SetActive(true);
                        UpdateChest(ui.chest5x5UI.GetComponent<ChestUI>());
                    }
                    ui.OpenInventory();
                }
                else
                {
                    ui.CloseInventory();
                    ui.chest5x5UI.SetActive(false);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            triggered = true;
        }
    }

    public void UpdateChest(ChestUI chestUI)
    {
        foreach (Transform item in chestUI.items.transform)
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < itemsIn.Count; i++)
        {
            if (itemsIn[i].value <= 0)
            {
                itemsIn.RemoveAt(i);
                UpdateChest(chestUI);
                return;
            }
            GameObject obj = Instantiate(chestUI.item, chestUI.items.transform);
            ChestItem itm = obj.GetComponent<ChestItem>();
            itm.inChestID = i;
            if (itemsIn[i].value > 1)
            {
                itm.text.text = itemsIn[i].value.ToString();
            }
            else
            {
                itm.text.text = "";
            }
            itm.image.sprite = FindObjectOfType<WorldManager>().GetItemBySecondName(itemsIn[i].secondName).icon;
            itm.gameObject.SetActive(true);
        }
    }
    public void AddItemChest(ChestUI chestUI, Item item)
    {
        bool finded = false;
        int value = item.value;
        var inventory = itemsIn;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].secondName == item.secondName && inventory[i].value < inventory[i].maxValue)
            {
                finded = true;
                for (int g = 0; g < item.value; g++)
                {
                    if (inventory[i].value < inventory[i].maxValue)
                    {
                        value--;
                        inventory[i].value++;
                    }
                    if (value == 0)
                    {
                        return;
                    }
                    if (inventory[i].value == inventory[i].maxValue && value > 0)
                    {
                        item.value = value;
                        AddItemChest(chestUI, item);
                        return;
                    }
                }
            }
        }
        if (finded == false)
        {
            if (itemsIn.Count < 25)
            {
                itemsIn.Add(item);
            }
            else
            {
                ui.AddItem(item);
            }
        }
        UpdateChest(chestUI);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            triggered = false;
            ui.selectedChest = null;
            if (type == ChestType.x5)
            {
                ui.chest5x5UI.SetActive(false);
            }
        }
    }
}

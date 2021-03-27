using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public GameObject mainCanvas;
    public PlayerStats playerStats;
    public List<int> hotBarItems = new List<int>(10);
    public List<InventoryItem> hotBarItemsScripts = new List<InventoryItem>(10);
    public Transform hotBarHolder;
    [Space]
    public int selectedCase;
    [Space]
    public Color selectedColor, normalColor;
    public Sprite UIItemImage;
    public List<Transform> hotBarItemsUI;

    public GameObject itemsHolder, itemClone;
    public GameObject craftHolder, craftClone;

    public Image[] hearts;
    public TMPro.TMP_Text armorText;

    public GameObject inventoryWindow, inventoryHotBar;

    public GameObject craftsListItems, craftsListContent;


    public Sprite InvIcon, CrafIcon, AnvIcon, ForgIcon, PotIcon;


    public GameObject escapeMenu;

    public GameObject selectedChest;

    public GameObject chest5x5UI;

    public Item mouseItem;

    public InventoryItem mouseItemPreview;

    public bool pause;

    public GameObject[] allWindows;

    public TMPro.TMP_Text hpCounter;
    public GameObject errorCanvas;

    [Header("Свойства итема")]
    public TMP_Text svHeader;
    public TMP_Text svDesc;
    public Image svImage;
    public GameObject hint;

    [Header("Инфо при наведении о крафте")]
    public TMP_Text infHeader;
    public TMP_Text infDesc, infNoCraft;
    public Image infImage;
    public Transform infItem;
    public Transform infContent;
    public GameObject infoWindow;
    public BuildDig buildDig;

    [Header("Список добавленного")]
    public Transform addContent;
    public Transform additem;
    public GameObject additemwiget;
    public IEnumerator additemhider;
    [Header("Выбранные стрелы и поиск инфо")]
    public SelectArrows selectArrows;
    public FindCraft findCraft;
    public TMP_Text infoText;

    [Space]
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    public static string Reverse(string stringToReverse)
    {
        char[] stringArray = stringToReverse.ToCharArray();
        string reverse = System.String.Empty;

        System.Array.Reverse(stringArray);

        return new string(stringArray);
    }
    public void AddItemView(Item item)
    {
        additemwiget.SetActive(true);
        if (addContent.childCount > 0)
        {
            if (addContent.GetChild(addContent.childCount - 1).GetComponentInChildren<Image>().sprite == item.icon)
            {
                var txt = addContent.GetChild(addContent.childCount - 1).GetComponentInChildren<TMP_Text>();
                string count = "";
                for (int i = txt.text.Length - 1; i > 0; i--)
                {
                    if (txt.text[i] == 'x') break;
                    count += txt.text[i];
                }
                txt.text = item.name + " x" + (int.Parse(Reverse(count)) + item.value);
                return;
            }
        }
        var g = Instantiate(additem.gameObject, addContent);
        g.GetComponentInChildren<TMP_Text>().text = item.name + " x" + item.value;
        g.GetComponentInChildren<Image>().sprite = item.icon;
        g.SetActive(true);
        if (additemhider != null)
            StopCoroutine(additemhider);
        additemhider = waitToHide(5f);
        StartCoroutine(additemhider);
    }
    IEnumerator waitToHide(float time)
    {
        yield return new WaitForSeconds(time);
        additemwiget.SetActive(false);
    }
    public void setCraftInfo(string secondname)
    {
        infNoCraft.gameObject.SetActive(false);
        var m = FindObjectOfType<WorldManager>();
        for (int i = 0; i < m.crafts.Count; i++)
        {
            if (secondname == m.crafts[i].finalItem)
            {
                Item finalItem = m.GetItemBySecondName(secondname);
                infImage.sprite = finalItem.icon;
                infHeader.text = finalItem.name;
                infDesc.text = FindObjectOfType<WorldConfig>().config.eng ? finalItem.descriptionen : finalItem.descriptionru;
                foreach (Transform item in infContent)
                {
                    Destroy(item.gameObject);
                }
                GameObject subc = Instantiate(infItem.gameObject, infContent);
                if (m.crafts[i].craftType == "Inventory")
                {
                    subc.transform.Find("Image").GetComponent<Image>().sprite = InvIcon;
                }
                if (m.crafts[i].craftType == "CraftingTable")
                {
                    subc.transform.Find("Image").GetComponent<Image>().sprite = CrafIcon;
                }
                if (m.crafts[i].craftType == "Anvil")
                {
                    subc.transform.Find("Image").GetComponent<Image>().sprite = AnvIcon;
                }
                if (m.crafts[i].craftType == "Forge")
                {
                    subc.transform.Find("Image").GetComponent<Image>().sprite = ForgIcon;
                }
                if (m.crafts[i].craftType == "Potions")
                {
                    subc.transform.Find("Image").GetComponent<Image>().sprite = PotIcon;
                }
                if (FindObjectOfType<WorldConfig>().config.eng == false)
                {
                    subc.transform.GetComponentInChildren<TMP_Text>().text = "< Нужен";
                }
                else
                {
                    subc.transform.GetComponentInChildren<TMP_Text>().text = "< Use it";
                }
                subc.SetActive(true);
                for (int g = 0; g < m.crafts[i].crafts.Count; g++)
                {
                    var p = m.GetItemBySecondName(m.crafts[i].crafts[g].item);
                    GameObject inst = Instantiate(infItem.gameObject, infContent);
                    inst.transform.Find("Image").GetComponentInChildren<Image>().sprite = p.icon;
                    inst.GetComponentInChildren<TMP_Text>().text = p.name + " - " + m.crafts[i].crafts[g].value;
                    inst.SetActive(true);

                }
                return;
            }
        }

        Item fItem = m.GetItemBySecondName(secondname);
        if (fItem != null)
        {
            infImage.sprite = fItem.icon;
            infHeader.text = fItem.name;
            infDesc.text = FindObjectOfType<WorldConfig>().config.eng ? fItem.descriptionen : fItem.descriptionru;
            foreach (Transform item in infContent)
            {
                Destroy(item.gameObject);
            }
            infNoCraft.gameObject.SetActive(true);
        }
    }

    public void SetProperties(Item item)
    {
        svHeader.text = item.name + " " + item.value + "/" + item.maxValue;
        if (FindObjectOfType<WorldConfig>().config.eng)
        {
            svDesc.text = item.descriptionen;
            if (item.type == Item.itemType.Dig || item.type == Item.itemType.Axe)
            {
                svDesc.text += "\nDig level: " + item.digLevel;
            }
            if (item.type == Item.itemType.Body || item.type == Item.itemType.Helmet || item.type == Item.itemType.Hands)
            {
                svDesc.text += "\nDig level: " + item.armor;
                svDesc.text += "\nFor use, pick up and use.";
            }
            if (item.type == Item.itemType.Melee)
            {
                svDesc.text += "\nSpeed: " +  item.coolDown;
                svDesc.text += "\nAttack: " + item.damage;
            }
            if (item.type == Item.itemType.Distance)
            {
                svDesc.text += "\nSpeed: " + item.coolDown;
            }
            if (item.type == Item.itemType.Use)
            {
                svDesc.text += "\nYou can use this";
            }
            if (item.type == Item.itemType.Backpack)
            {
                var sp = item.data.Split(';').ToArray();
                for (int i = 0; i < sp.Length; i++)
                {
                    var sp1 = sp[i].Split(':');
                }

                for (int i = 0; i < sp.Length; i++)
                {
                    var sp1 = sp[i].Split(':');

                    if (sp1.Contains("light"))
                    {
                        svDesc.text += "\nLight at a distance: " + sp1[1];
                    }
                    if (sp1.Contains("fly"))
                    {
                        svDesc.text += "\nReduces gravity by: " + sp1[1];
                    }
                    if (sp1.Contains("speed"))
                    {
                        svDesc.text += "\nIncreases Speed to: " + sp1[1];
                    }
                }
            }
        }
        else
        {
            svDesc.text = item.descriptionru;

            if (item.type == Item.itemType.Dig || item.type == Item.itemType.Axe)
            {
                svDesc.text += "\nМощность: " + item.digLevel;
            }
            if (item.type == Item.itemType.Body || item.type == Item.itemType.Helmet || item.type == Item.itemType.Hands)
            {
                svDesc.text += "\nМощность: " + item.armor;
                svDesc.text += "\nДля применения, возьмите в руки и используйте.";
            }
            if (item.type == Item.itemType.Melee)
            {
                svDesc.text += "\nСкрость: " + item.coolDown;
                svDesc.text += "\nСила: " + item.damage;
            }
            if (item.type == Item.itemType.Distance)
            {
                svDesc.text += "\nСкрость: " + item.coolDown;
            }
            if (item.type == Item.itemType.Use)
            {
                svDesc.text += "\nМожешь использовать это";
            }
            if (item.type == Item.itemType.Backpack)
            {
                var sp = item.data.Split(';').ToArray();
                for (int i = 0; i < sp.Length; i++)
                {
                    var sp1 = sp[i].Split(':');
                }

                for (int i = 0; i < sp.Length; i++)
                {
                    var sp1 = sp[i].Split(':');

                    if (sp1.Contains("light"))
                    {
                        svDesc.text += "\nИзлучает на расстояние: " + sp1[1];
                    }
                    if (sp1.Contains("fly"))
                    {
                        svDesc.text += "\nУменьшает гравитацию на: " + sp1[1];
                    }
                    if (sp1.Contains("speed"))
                    {
                        svDesc.text += "\nУвеличивает скорость на: " + sp1[1];
                    }
                }
            }
        }
        svImage.sprite = item.icon;
    }

    public void CraftGuideUpdate()
    {
        foreach (Transform item in craftsListContent.transform)
        {
            Destroy(item.gameObject);
        }
        var m = FindObjectOfType<WorldManager>();
        for (int i = 0; i < m.crafts.Count; i++)
        {
            GameObject inst = Instantiate(craftsListItems, craftsListContent.transform);
            CraftsListItem crf = inst.GetComponent<CraftsListItem>();
            crf.ID = m.crafts[i].finalItem;
            Item finalItem = m.GetItemBySecondName(crf.ID);
            crf.image.sprite = finalItem.icon;
            crf.craftItemName.text = finalItem.name;


            var itc = m.GetItemBySecondName(m.crafts[i].finalItem);
            GameObject subc = Instantiate(crf.subCraft, crf.subCraftsContent.transform);
            if (m.crafts[i].craftType == "Inventory")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = InvIcon;
            }
            if (m.crafts[i].craftType == "CraftingTable")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = CrafIcon;
            }
            if (m.crafts[i].craftType == "Anvil")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = AnvIcon;
            }
            if (m.crafts[i].craftType == "Forge")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = ForgIcon;
            }
            if (m.crafts[i].craftType == "Potions")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = PotIcon;
            }
            TextTranslator p = subc.transform.Find("Name").gameObject.AddComponent<TextTranslator>();
            p.engText = "< Use it";
            p.rusText = "< Нужен";
            p.TMPText = subc.transform.Find("Name").GetComponent<TMPro.TMP_Text>();

            subc.transform.Find("Value").GetComponent<TMPro.TMP_Text>().text = "";
            subc.SetActive(true);

            for (int x = 0; x < m.crafts[i].crafts.Count; x++)
            {
                var it = m.GetItemBySecondName(m.crafts[i].crafts[x].item);
                GameObject sub = Instantiate(crf.subCraft, crf.subCraftsContent.transform);
                sub.GetComponent<CraftSubItem>().ID = it.secondName;
                sub.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = it.icon;
                sub.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text = it.name;
                sub.transform.Find("Value").GetComponent<TMPro.TMP_Text>().text = m.crafts[i].crafts[x].value.ToString();
                sub.SetActive(true);
            }
            inst.SetActive(true);
        }
    }

    public void CraftGuideUpdate(string itemname)
    {
        if (itemname == "")
        {
            CraftGuideUpdate();
            return;
        }

        foreach (Transform item in craftsListContent.transform)
        {
            Destroy(item.gameObject);
        }
        var m = FindObjectOfType<WorldManager>();
        for (int i = 0; i < m.crafts.Count; i++)
        {
            int craftsFind = 0;

            if (m.crafts[i].finalItem == itemname) craftsFind++;

            GameObject inst = Instantiate(craftsListItems, craftsListContent.transform);
            CraftsListItem crf = inst.GetComponent<CraftsListItem>();
            crf.ID = m.crafts[i].finalItem;
            Item finalItem = m.GetItemBySecondName(crf.ID);
            crf.image.sprite = finalItem.icon;
            crf.craftItemName.text = finalItem.name;


            GameObject subc = Instantiate(crf.subCraft, crf.subCraftsContent.transform);
            if (m.crafts[i].craftType == "Inventory")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = InvIcon;
            }
            if (m.crafts[i].craftType == "CraftingTable")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = CrafIcon;
            }
            if (m.crafts[i].craftType == "Anvil")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = AnvIcon;
            }
            if (m.crafts[i].craftType == "Forge")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = ForgIcon;
            }
            if (m.crafts[i].craftType == "Potions")
            {
                subc.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = PotIcon;
            }
            TextTranslator p = subc.transform.Find("Name").gameObject.AddComponent<TextTranslator>();
            p.engText = "< Use it";
            p.rusText = "< Нужен";
            p.TMPText = subc.transform.Find("Name").GetComponent<TMPro.TMP_Text>();

            subc.transform.Find("Value").GetComponent<TMPro.TMP_Text>().text = "";
            subc.SetActive(true);
            for (int x = 0; x < m.crafts[i].crafts.Count; x++)
            {
                var it = m.GetItemBySecondName(m.crafts[i].crafts[x].item);
                if (it.secondName == itemname)
                {
                    craftsFind++;
                }
            }
            if (craftsFind != 0)
            {
                for (int x = 0; x < m.crafts[i].crafts.Count; x++)
                {
                    var it = m.GetItemBySecondName(m.crafts[i].crafts[x].item);
                    GameObject sub = Instantiate(crf.subCraft, crf.subCraftsContent.transform);
                    sub.GetComponent<CraftSubItem>().ID = it.secondName;
                    sub.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = it.icon;
                    sub.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text = it.name;
                    sub.transform.Find("Value").GetComponent<TMPro.TMP_Text>().text = m.crafts[i].crafts[x].value.ToString();
                    sub.SetActive(true);
                }
                inst.SetActive(true);
            }
            else
            {
                Destroy(inst.gameObject);
            }
        }
    }


    public void UpdateHotBarBack()
    {
        for (int j = 0; j < 9; j++)
        {
            hotBarItemsUI[j].GetComponent<Image>().color = normalColor;
            hotBarItemsUI[j].localScale = new Vector3(1, 1, 1);
        }
        hotBarItemsUI[selectedCase].GetComponent<Image>().color = selectedColor;
        hotBarItemsUI[selectedCase].localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    private void Start()
    {
        for (int i = 1; i < 10; i++)
        {
            GameObject image = new GameObject();
            image.transform.parent = hotBarHolder;
            image.AddComponent<Image>();
            image.GetComponent<Image>().sprite = UIItemImage;
            image.GetComponent<Image>().color = normalColor;
            hotBarItems.Add(-1);
            hotBarItemsUI.Add(image.transform);
            if (i == 1)
            {
                image.GetComponent<Image>().color = selectedColor;
                selectedCase = 0;
            }
            GameObject it = Instantiate(itemClone.gameObject, inventoryHotBar.transform);
            it.AddComponent<HotBarMouseClick>().id = i-1;
            var c = it.GetComponent<InventoryItem>();
            c.canMouseOver = false;
            it.SetActive(true);
            hotBarItemsScripts.Add(c);
        }
        hotBarItems.Add(-1);

        ///Inventory items
        for (int i = 0; i < playerStats.localPlayer.inventorySize; i++)
        {
            GameObject it = Instantiate(itemClone.gameObject, itemsHolder.transform);
            var c = it.GetComponent<InventoryItem>();
            c.stats = playerStats;
            inventoryItems.Add(c);
            it.gameObject.SetActive(false);
        }


        CraftGuideUpdate();
        UpdateInventory(true);
        UpdateHotbar();
        HealthUpdate();
        for (int i = 0; i < 9; i++)
        {
            UpdateHotbar();
            for (int j = 0; j < 9; j++)
            {
                hotBarItemsUI[j].GetComponent<Image>().color = normalColor;
                hotBarItemsUI[j].localScale = new Vector3(1, 1, 1);
            }
            hotBarItemsUI[0].GetComponent<Image>().color = selectedColor;
            hotBarItemsUI[0].localScale = new Vector3(1.2f, 1.2f, 1.2f);
            selectedCase = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!mainCanvas.active)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                mainCanvas.active = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
            GetComponent<PlayerStart>().F1.active = !GetComponent<PlayerStart>().F1.active;
        if (Input.GetKeyDown(KeyCode.Mouse0))
            GetComponent<PlayerStart>().F1.active = false;

        buildDig.enabled = !allWindows[2].active;

        hpCounter.text = "HP: " + playerStats.localPlayer.health + "/" + (playerStats.localPlayer.stdHealth + playerStats.localPlayer.dopHealth);
        armorText.text = "x" + playerStats.localPlayer.armor;
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (inventoryWindow.active == false)
            {
                if (hotBarItems[selectedCase] != -1)
                {
                    //playerStats.localPlayer.inventory[hotBarItems[selectedCase]].inInventory = -1;
                    UpdateInventory(true);
                    UpdateHotbar();
                    return;
                }
            }
        }
        
    }

    public void UpdateHotbar()
    {
        for (int p = 0; p < hotBarItems.Count-1; p++)
        {
            hotBarItems[p] = -1;
            for (int i = 0; i < playerStats.localPlayer.inventory.Count; i++)
            {
                if (playerStats.localPlayer.inventory[i].inInventory == p)
                {
                    hotBarItems[p] = i;
                    var c = hotBarItemsScripts[p];
                    //c.inventoryIndex = hotBarItems[i];
                    c.image.gameObject.SetActive(true);
                    c.stats = playerStats;
                    c.image.sprite = playerStats.localPlayer.inventory[hotBarItems[p]].icon;
                    if (playerStats.localPlayer.inventory[hotBarItems[p]].value > 1)
                    {
                        c.text.text = playerStats.localPlayer.inventory[hotBarItems[p]].value.ToString();
                    }
                    else
                    {
                        c.text.text = "";
                    }
                    break;
                }
            }
            if (hotBarItems[p] == -1)
            {
                hotBarItemsScripts[p].image.gameObject.SetActive(false);
                hotBarItemsScripts[p].text.text = "";
            }

        }
    }

    public void HealthUpdate()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].color = new Color(1, 1, 1, 0.2f);
        }
        for (int i = 0; i < (playerStats.localPlayer.health) / 20; i++)
        {
            hearts[i].color = new Color(1, 1, 1, 1);

        }
        if (playerStats.localPlayer.health < 20 && playerStats.localPlayer.health != 0)
        {
            hearts[0].color = new Color(1, 1, 1, 1);
        }
    }

    public void OpenInventory()
    {
        inventoryWindow.SetActive(true);
        pause = inventoryWindow.active;
        if (pause == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
        UpdateInventory(true);
        UpdateHotbar();
    }
    public void CloseInventory()
    {
        inventoryWindow.SetActive(false);
        pause = inventoryWindow.active;
        if (pause == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            if (escapeMenu.active == false)
            {
                Time.timeScale = 1;
            }
        }
        UpdateHotbar();
    }

    private void Update()
    {
        {//Кнопки
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (mouseItem.secondName != "_None_")
                {
                    print("drop");
                    DropItem(mouseItem, false);
                    mouseItem = new Item();
                    UpdateInventory(true);
                    UpdateHotbar();
                }
            }
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    UpdateHotbar();
                    for (int j = 0; j < 9; j++)
                    {
                        hotBarItemsUI[j].GetComponent<Image>().color = normalColor;
                        hotBarItemsUI[j].localScale = new Vector3(1, 1, 1);
                    }
                    hotBarItemsUI[i].GetComponent<Image>().color = selectedColor;
                    hotBarItemsUI[i].localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    selectedCase = i;
                }
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    UpdateHotbar();
                    for (int j = 0; j < 9; j++)
                    {
                        hotBarItemsUI[j].GetComponent<Image>().color = normalColor;
                        hotBarItemsUI[j].localScale = new Vector3(1, 1, 1);
                    }
                    selectedCase = 9;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            for (int i = 0; i < allWindows.Length; i++)
            {
                if (allWindows[i].active == true)
                {
                    if (allWindows[i] == inventoryWindow)
                    {
                        if (escapeMenu.active == false)
                        {
                            Time.timeScale = 1;
                        }
                    }
                    allWindows[i].active = false;
                    return;
                }
            }

            if (GetComponent<Console>().input.active == false)
            {
                pause = !pause;
                if (pause == true)
                {
                    escapeMenu.SetActive(true);
                    Time.timeScale = 0f;
                }
                else
                {
                    if (inventoryWindow.active == false)
                    {
                        escapeMenu.SetActive(false);
                        Time.timeScale = 1;
                    }
                    else
                    {
                        Time.timeScale = 0f;
                    }
                }
            }
            if (GetComponent<Console>().input.active == true)
            {

                GetComponent<Console>().input.active = false;
                GetComponent<Console>().Close();
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryWindow.SetActive(!inventoryWindow.active);
            pause = inventoryWindow.active;
            if (pause == true)
            {
                Time.timeScale = 0f;
            }
            else
            {
                if (escapeMenu.active == false)
                {
                    Time.timeScale = 1;
                }
            }
            UpdateInventory(true);
        }
        
        if (inventoryWindow.active)
        {
            if (mouseItem.secondName != "_None_")
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (selectArrows.over)
                    {
                        if (mouseItem.data.Contains("bullet"))
                        {
                            buildDig.selectedArrows = mouseItem.secondName;
                        }
                    }
                    if (findCraft.over)
                    {
                        findCraft.item = mouseItem.secondName;
                    }
                    AddItem(mouseItem);
                    mouseItem = new Item();
                    UpdateInventory(true);
                    mouseItemPreview.gameObject.SetActive(false);
                    return;
                }
                mouseItemPreview.gameObject.SetActive(true);
                mouseItemPreview.image.sprite = mouseItem.icon;
                mouseItemPreview.text.text = mouseItem.value.ToString();
                mouseItemPreview.transform.position = Input.mousePosition;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (selectArrows.over)
                    {
                        buildDig.selectedArrows = "";
                    }
                    if (findCraft.over)
                    {
                        findCraft.item = "";
                    }
                }
                mouseItemPreview.gameObject.SetActive(false);
            }
        }
        else
        {
            mouseItemPreview.gameObject.SetActive(false);
            if (mouseItem.secondName != "_None_")
            {
                AddItem(mouseItem);
                mouseItem = new Item();
            }
        }
    }
    public void UpdateCraft()
    {

        if (craftHolder.transform.childCount > 0)
        {
            foreach (Transform child in craftHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }
        var manager = FindObjectOfType<WorldManager>();
        for (int i = 0; i < manager.crafts.Count; i++)
        {
            for (int p = 0; p < playerStats.localPlayer.craftingTables.Count; p++)
            {
                if (manager.crafts[i].craftType == playerStats.localPlayer.craftingTables[p])
                {
                    int items = 0;
                    for (int j = 0; j < manager.crafts[i].crafts.Count; j++)
                    {
                        for (int k = 0; k < playerStats.localPlayer.inventory.Count; k++)
                        {
                            if (playerStats.localPlayer.inventory[k].secondName == manager.crafts[i].crafts[j].item)
                            {
                                if (playerStats.localPlayer.inventory[k].value >= manager.crafts[i].crafts[j].value)
                                {
                                    items++;
                                }
                            }
                        }
                    }
                    if (items == manager.crafts[i].crafts.Count)
                    {
                        GameObject it = Instantiate(craftClone.gameObject, craftHolder.transform);
                        it.GetComponent<CraftItem>().image.sprite = manager.GetItemBySecondName(manager.crafts[i].finalItem).icon;

                        it.GetComponent<CraftItem>().craftID = i;
                        it.GetComponent<CraftItem>().stats = playerStats;
                        it.gameObject.SetActive(true);
                        break;
                    }
                }

            }
        }
    }
    public void UpdateInventory(bool updateCraft)
    {
        ///Удаление _Ноне_
        for (int i = 0; i <  playerStats.localPlayer.inventory.Count; i++)
        {
            if (playerStats.localPlayer.inventory[i].secondName == "_None_")
            {
                playerStats.localPlayer.inventory.RemoveAt(i);
                UpdateInventory(updateCraft);
                return;
            }

        }

        for (int i = 0; i < inventoryItems.Count; i++)
        {

            if (i < playerStats.localPlayer.inventory.Count)
            {
                if (playerStats.localPlayer.inventory[i].value <= 0)
                {
                    playerStats.localPlayer.inventory.RemoveAt(i);
                    UpdateInventory(updateCraft);
                    return;
                }
                if (playerStats.localPlayer.inventory[i].secondName != "_None_")
                {
                    var c = inventoryItems[i];
                    c.inventoryIndex = i;
                    c.stats = playerStats;
                    c.Init();

                    if (c.gameObject.active == false)
                    {
                        c.mouseOver = false;
                    }
                    c.canMouseOver = true;
                    inventoryItems[i].gameObject.SetActive(true);
                }
            }
            else
            {
                inventoryItems[i].gameObject.SetActive(false);
            }
        }
        if (updateCraft)
            UpdateCraft();
    }

    public void PauseOFF()
    {
        escapeMenu.SetActive(false);
        if (!inventoryWindow.active)
        {
            pause = false;
            Time.timeScale = 1;
        }
    }


    public void AddItem(Item item, bool updateCraft = true)
    {
        bool finded = false;
        int value = item.value;
        int oldValue = 0;
        var inventory = playerStats.localPlayer.inventory.FindAll(x => x.secondName == item.secondName);
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].value < inventory[i].maxValue)
            {
                finded = true;
                for (int g = 0; g < item.value; g++)
                {
                    if (inventory[i].value < inventory[i].maxValue)
                    {
                        value--;
                        inventory[i].value++;
                        oldValue++;
                    }
                    if (value <= 0)
                    {
                        UpdateInventory(updateCraft);
                        UpdateHotbar();
                        ///
                        var p = Item.Clone(item);
                        p.value = oldValue;
                        AddItemView(p);
                        ///
                        return;
                    }
                    if (inventory[i].value == inventory[i].maxValue && value > 0)
                    {
                        item.value = value;
                        AddItem(item, updateCraft);
                        ////
                        var p = Item.Clone(item);
                        p.value = oldValue;
                        AddItemView(p);
                        ///
                        return;
                    }
                }
            }
            else continue;
        }
        print(finded);
        if (finded == false)
        {
            if (playerStats.localPlayer.inventory.Count >= playerStats.localPlayer.inventorySize)
            {
                Drop drop = Instantiate(FindObjectOfType<WorldManager>().drop, transform.position, Quaternion.identity).GetComponent<Drop>();
                drop.item = item;
                drop.Init();
                UpdateInventory(updateCraft);
                UpdateHotbar();
                return;
            }
            ///
            var p = Item.Clone(item);
            AddItemView(p);
            ///
            int emptyhotbar = -1;
            for (int i = 0; i < hotBarItems.Count; i++)
            {
                if (hotBarItems[i] == -1 && i != selectedCase)
                {
                    emptyhotbar = i;
                    break;
                }
            }
            if (emptyhotbar != -1)
            {
                item.inInventory = emptyhotbar;
            }
            playerStats.localPlayer.inventory.Add(item);
        }
        UpdateInventory(updateCraft);
        UpdateHotbar();
    }

    public bool FindItemInInventoryByName(Item item)
    {
        for (int i = 0; i < playerStats.localPlayer.inventory.Count; i++)
        {
            if (playerStats.localPlayer.inventory[i].secondName == item.secondName)
            {
                return true;
            }
        }
        return false;
    }

    public bool FindItemInInventoryByName(string item)
    {
        for (int i = 0; i < playerStats.localPlayer.inventory.Count; i++)
        {
            if (playerStats.localPlayer.inventory[i].secondName == item)
            {
                return true;
            }
        }
        return false;
    }
    public Item GetItemInInventoryByName(Item item)
    {
        for (int i = 0; i < playerStats.localPlayer.inventory.Count; i++)
        {
            if (playerStats.localPlayer.inventory[i].secondName == item.secondName)
            {
                return playerStats.localPlayer.inventory[i];
            }
        }
        return null;
    }
    public Item GetItemInInventoryByData(string dataContains)
    {
        for (int i = 0; i < playerStats.localPlayer.inventory.Count; i++)
        {
            if (playerStats.localPlayer.inventory[i].data.Contains(dataContains))
            {
                return playerStats.localPlayer.inventory[i];
            }
        }
        return null;
    }
    #region oldAdd
    //public void AddItem(Item item)
    //{
    //    bool added = false;
    //    bool createNew = false;
    //    int canToStack = 0;
    //    for (int i = 0; i < playerStats.localPlayer.inventory.Count; i++)
    //    {
    //        if (playerStats.localPlayer.inventory[i].secondName == item.secondName)
    //        {
    //            if (playerStats.localPlayer.inventory[i].value < playerStats.localPlayer.inventory[i].maxValue)
    //            {
    //                added = true;
    //                for (int v = 0; v < item.value; v++)
    //                {
    //                    if (playerStats.localPlayer.inventory[i].value < playerStats.localPlayer.inventory[i].maxValue)
    //                    {
    //                        item.value -= 1;
    //                        playerStats.localPlayer.inventory[i].value++;
    //                    }
    //                    else
    //                    {
    //                        for (int s = 0; s < playerStats.localPlayer.inventory.Count; s++)
    //                        {
    //                            if (playerStats.localPlayer.inventory[i].secondName == item.secondName)
    //                            {
    //                                canToStack += 1;
    //                            }
    //                        }
    //                        if (canToStack > 0)
    //                        {
    //                            createNew = false;
    //                            added = false;
    //                            AddItem(item);
    //                            break;
    //                        }
    //                        else
    //                        {
    //                            createNew = true;
    //                            break;
    //                        }
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    if (added == false || createNew == true)
    //    {
    //        playerStats.localPlayer.inventory.Add(item);
    //    }

    //    UpdateInventory();
    //}
    #endregion
    public void RemoveItem(Item item, bool updateCraft = true)
    {
        for (int i = 0; i < playerStats.localPlayer.inventory.Count; i++)
        {
            if (playerStats.localPlayer.inventory[i].secondName == item.secondName)
            {
                if (playerStats.localPlayer.inventory[i].value >= item.value)
                {
                    playerStats.localPlayer.inventory[i].value -= item.value;
                    if (playerStats.localPlayer.inventory[i].value <= 0)
                    {
                        playerStats.localPlayer.inventory.RemoveAt(i);
                    }
                    break;
                }
            }
        }
        UpdateInventory(updateCraft);
        UpdateHotbar();
    }
    public void ButtonActive(GameObject gameObject)
    {
        gameObject.active = !gameObject.active;
        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
    }

    public void DropItem(Item item, bool sub)
    {
        item.inInventory = -1;
        if (sub)
        {
            RemoveItem(item);
        }
        GameObject obj = Instantiate(FindObjectOfType<WorldManager>().drop, transform.position, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * 1000 * Time.deltaTime);
        if (transform.localScale.x > 0)
        {
            obj.transform.position = transform.position + new Vector3(2, 0, 0);
            obj.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.right * 1000 * Time.deltaTime);
        }
        if (transform.localScale.x < 0)
        {
            obj.transform.position = transform.position - new Vector3(2, 0, 0);
            obj.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.left * 1000 * Time.deltaTime);
        }
        obj.GetComponent<Drop>().item = item;
        obj.GetComponent<Drop>().Init();
    }
}

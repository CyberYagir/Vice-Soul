using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Serializable]
public class ItemManagerWindow : EditorWindow
{
    [MenuItem("Window/General/Developer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ItemManagerWindow), false, "Developer");
    }

    public string itemName, description, secondName;
    public Item.itemType type = Item.itemType.None;
    public Sprite icon;
    public int value = 1, maxValue = 999;

    public TileBase block;
    public Vector2Int intSize = new Vector2Int(1,1);
    public Vector2 floatSize = new Vector2(1,1);

    public float digLevel;

    public GameObject prefab;

    public int tab = 0;
    Vector2 scroll;
    int spriteSize = 100;
    public int buttonHeight = 30;
    public Texture2D background;
    public bool openItems, insert;
    public int insertValue = 0;
    public float distance, coolDown;
    public int damage;
    public WorldManager worldManager;


    public string finalItem;
    public enum craftTypes {Inventory, Forge, CraftingTable, Anvil};
    public craftTypes craftType;
    public List<WorldManager.Craft.CraftItem> itemsToCraft = new List<WorldManager.Craft.CraftItem>();
    public bool seeCrafts = false;


    void OnGUI()
    {
        worldManager = FindObjectOfType<WorldManager>();
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), background, ScaleMode.ScaleAndCrop);
        EditorStyles.textArea.wordWrap = true;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Create:", EditorStyles.miniButton, GUILayout.Height(buttonHeight), GUILayout.MaxWidth(70));

        tab = GUILayout.Toolbar(tab, new string[] {"Item", "Craft", "Block"});

        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (tab == 0)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            itemName = EditorGUILayout.TextField("Item name:", itemName);
            type = (Item.itemType)EditorGUILayout.EnumPopup("Type:", type);
            secondName = EditorGUILayout.TextField("ID: ", secondName);
            description = EditorGUILayout.TextField("Description: ", description);
            if (type == Item.itemType.Block)
            {
                block = (TileBase)EditorGUILayout.ObjectField("Tile:", block, typeof(TileBase), true);
                intSize = EditorGUILayout.Vector2IntField("Block preview size:", intSize);
            }
            else
            {
                icon = (Sprite)EditorGUILayout.ObjectField("Icon:", icon, typeof(Sprite), true, GUILayout.Height(spriteSize));
            }
            if (type == Item.itemType.Table)
            {
                prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", prefab, typeof(GameObject), true);
                intSize = EditorGUILayout.Vector2IntField("Block preview size:", intSize);
            }
            value = EditorGUILayout.IntSlider("Value:", value, 1, 100);
            maxValue = EditorGUILayout.IntSlider("Max Value:", maxValue, 1, 999);
            
            if (type == Item.itemType.Dig || type == Item.itemType.Axe || type == Item.itemType.Distance || type == Item.itemType.Hammer)
            {
                EditorGUILayout.LabelField("Eqipent:", GUILayout.Height(20));

                floatSize = EditorGUILayout.Vector2Field("Weapon size:", floatSize);
                digLevel = EditorGUILayout.FloatField("Dig Level:", digLevel);
            }
            if (type == Item.itemType.Melee)
            {
                floatSize = EditorGUILayout.Vector2Field("Weapon size: ", floatSize);
                distance = EditorGUILayout.FloatField("Distance:", distance);
                damage = EditorGUILayout.IntField("Damage: ", damage);
                coolDown = EditorGUILayout.FloatField("CoolDown: ", coolDown);

            }
            if (GUILayout.Button("Insert into: " + insert, EditorStyles.miniButton, GUILayout.Width(100)))
            {
                insert = !insert;
            }
            if (insert)
            {
                insertValue = EditorGUILayout.IntSlider("Insert into value:", insertValue, 0, worldManager.items.Count);
            }
            if (GUILayout.Button("See items: " + openItems, EditorStyles.miniButton, GUILayout.Width(100)))
            {
                openItems = !openItems;
            }
            if (openItems)
            {
                for (int i = 0; i < worldManager.items.Count; i++)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.LabelField(i.ToString());
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Name: " + worldManager.items[i].name);
                    EditorGUILayout.LabelField("ID:   " + worldManager.items[i].secondName);
                    EditorGUILayout.LabelField("Type: " + worldManager.items[i].type.ToString());
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Get values", EditorStyles.miniButtonLeft, GUILayout.Width(100)))
                    {
                        itemName = worldManager.items[i].name;
                        description = worldManager.items[i].descriptionru;
                        secondName = worldManager.items[i].secondName;
                        type = worldManager.items[i].type;
                        icon = worldManager.items[i].icon;
                        value = worldManager.items[i].value;
                        maxValue = worldManager.items[i].maxValue;
                        block = worldManager.items[i].tile;
                        intSize = worldManager.items[i].size;
                        floatSize = worldManager.items[i].floatSize;
                        digLevel = worldManager.items[i].digLevel;
                        prefab = worldManager.items[i].prefab;

                    }
                    if (GUILayout.Button("Remove", EditorStyles.miniButtonRight, GUILayout.Width(100))) {
                        worldManager.items.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    if (worldManager.items[i].type != Item.itemType.Block)
                    {
                        GUILayout.BeginVertical();
                        if (worldManager.items[i].type == Item.itemType.Table)
                        {
                            EditorGUILayout.ObjectField((GameObject)worldManager.items[i].prefab, typeof(GameObject), true);
                        }
                        EditorGUILayout.ObjectField(worldManager.items[i].icon, typeof(Sprite), true);
                        EditorGUILayout.ObjectField(worldManager.items[i].icon, typeof(Sprite), true, GUILayout.Height(spriteSize / 2), GUILayout.Width(spriteSize / 2));
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUILayout.BeginVertical();
                        EditorGUILayout.ObjectField((TileBase)worldManager.items[i].tile, typeof(TileBase), true);
                        EditorGUILayout.ObjectField(worldManager.items[i].icon, typeof(Sprite), true, GUILayout.Height(spriteSize / 2), GUILayout.Width(spriteSize / 2));



                        GUILayout.EndVertical();
                    }

                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset", EditorStyles.miniButtonLeft, GUILayout.Height(buttonHeight)))
            {
                itemName = "";
                description = "";
                secondName = "";
                type = Item.itemType.None;
                icon = null;
                value = 1;
                maxValue = 999;
                block = null;
                intSize = new Vector2Int(1, 1);
                floatSize = new Vector2(1, 1);
                digLevel = 0;
                prefab = null;
            }
            string CreateButtonName = "";
            if (worldManager.GetItemBySecondName(secondName) == null)
            {
                CreateButtonName = "Create item";
            }
            else
            {
                CreateButtonName = "Rewrite Item";
            }
            
            if (GUILayout.Button(CreateButtonName, EditorStyles.miniButtonRight, GUILayout.Height(buttonHeight)))
            {
                Item item = new Item();
                item.name = itemName;
                item.descriptionru = description;
                item.secondName = secondName;
                item.type = type;
                item.icon = icon;
                item.value = value;
                item.maxValue = maxValue;
                item.tile = block;
                item.size = intSize;
                item.floatSize = floatSize;
                item.digLevel = digLevel;
                item.prefab = prefab;
                item.distance = distance;
                item.damage = damage;
                item.coolDown = coolDown;
                if (worldManager.GetItemBySecondName(secondName) == null)
                {
                    if (insert)
                    {
                        worldManager.items.Insert(insertValue, item);
                    }
                    else
                    {
                        worldManager.items.Insert(0, item);
                    }
                }
                else
                {
                    int p = worldManager.GetItemIDBySecondName(secondName);
                    if (p != -1)
                    {
                        worldManager.items.RemoveAt(p);
                        if (insert)
                        {
                            worldManager.items.Insert(insertValue, item);
                        }
                        else
                        {
                            worldManager.items.Insert(p, item);
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        if (tab == 1)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            finalItem = EditorGUILayout.TextField("Final item: ", finalItem);
            if (worldManager.GetItemBySecondName(finalItem) != null)
            {
                icon = (Sprite)EditorGUILayout.ObjectField("Icon:", worldManager.GetItemBySecondName(finalItem).icon, typeof(Sprite), true, GUILayout.Height(spriteSize));
            }
            craftType = (craftTypes)EditorGUILayout.EnumPopup("Craft in: ", craftType);
            if (GUILayout.Button("Insert into: " + insert, EditorStyles.miniButton, GUILayout.Width(100)))
            {
                insert = !insert;
            }
            if (insert)
            {
                insertValue = EditorGUILayout.IntSlider("Insert into value:", insertValue, 0, worldManager.crafts.Count-1);
            }



            EditorGUILayout.LabelField("", GUI.skin.label);
            

            EditorGUILayout.LabelField("Items:");
            for (int i = 0; i < itemsToCraft.Count; i++)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.BeginHorizontal();
                    GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                            EditorGUILayout.LabelField("Name: ", worldManager.GetItemBySecondName(itemsToCraft[i].item).name);
                            itemsToCraft[i].item = EditorGUILayout.DelayedTextField("ID: ",itemsToCraft[i].item);
                            itemsToCraft[i].value = EditorGUILayout.IntSlider(itemsToCraft[i].value, 0, 999);
                        GUILayout.EndVertical();
                        icon = (Sprite)EditorGUILayout.ObjectField("Icon:", worldManager.GetItemBySecondName(itemsToCraft[i].item).icon, typeof(Sprite), true, GUILayout.Height(spriteSize/2));
                    GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Remove")) {
                    itemsToCraft.RemoveAt(i);
                };
            }
            EditorGUILayout.LabelField("", GUI.skin.label);
            EditorGUILayout.LabelField("", GUI.skin.label);
            EditorGUILayout.LabelField("", GUI.skin.label);

            if (GUILayout.Button("See items: " + openItems, EditorStyles.miniButton, GUILayout.Width(100)))
            {
                openItems = !openItems;
            }
            if (openItems)
            {
                for (int i = 0; i < worldManager.items.Count; i++)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.LabelField(i.ToString());
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Name: " + worldManager.items[i].name);
                    EditorGUILayout.LabelField("ID:   " + worldManager.items[i].secondName);
                    EditorGUILayout.LabelField("Type: " + worldManager.items[i].type.ToString());
                    EditorGUILayout.BeginHorizontal();

                    bool exist = false;
                    for (int p = 0; p < itemsToCraft.Count; p++)
                    {
                        if (worldManager.items[i].secondName == itemsToCraft[p].item)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        if (GUILayout.Button("Add to craft", EditorStyles.miniButton, GUILayout.Width(100)))
                        {
                            itemsToCraft.Add(new WorldManager.Craft.CraftItem() { item = worldManager.items[i].secondName, value = 1 });
                        }
                    }
                    if (GUILayout.Button("Final item", EditorStyles.miniButton, GUILayout.Width(100)))
                    {
                        finalItem = worldManager.items[i].secondName;
                    }


                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    if (worldManager.items[i].type != Item.itemType.Block)
                    {
                        GUILayout.BeginVertical();
                        EditorGUILayout.ObjectField(worldManager.items[i].icon, typeof(Sprite), true, GUILayout.Height(spriteSize / 2), GUILayout.Width(spriteSize / 2));
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUILayout.BeginVertical();
                        EditorGUILayout.ObjectField(worldManager.items[i].icon, typeof(Sprite), true, GUILayout.Height(spriteSize / 2), GUILayout.Width(spriteSize / 2));



                        GUILayout.EndVertical();
                    }

                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("See crafts: " + seeCrafts, EditorStyles.miniButton, GUILayout.Width(100)))
            {
                seeCrafts = !seeCrafts;
            }
            if (seeCrafts)
            {
                for (int i = 0; i < worldManager.crafts.Count; i++)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); 
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    for (int p = 0; p < worldManager.crafts[i].crafts.Count; p++)
                    {
                        var item = worldManager.GetItemBySecondName(worldManager.crafts[i].crafts[p].item);
                        EditorGUILayout.LabelField(item.name + ": " + worldManager.crafts[i].crafts[p].value + "/" + item.maxValue);
                        //EditorGUILayout.ObjectField(item.icon, typeof(Sprite), true, GUILayout.Height(spriteSize / 4), GUILayout.Width(spriteSize / 4));
                    }
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Get values", EditorStyles.miniButton))
                    {
                        finalItem = worldManager.crafts[i].finalItem;
                        if (worldManager.crafts[i].craftType == "Inventory")
                        {
                            craftType = craftTypes.Inventory;
                        }
                        if (worldManager.crafts[i].craftType == "Forge")
                        {
                            craftType = craftTypes.Forge;
                        }
                        if (worldManager.crafts[i].craftType == "CraftingTable")
                        {
                            craftType = craftTypes.CraftingTable;
                        }
                        if (worldManager.crafts[i].craftType == "Anvil")
                        {
                            craftType = craftTypes.Anvil;
                        }
                        itemsToCraft = worldManager.crafts[i].crafts;
                    }
                    if (GUILayout.Button("Delete", EditorStyles.miniButton))
                    {
                        worldManager.crafts.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Final Item: ");
                    EditorGUILayout.ObjectField(worldManager.GetItemBySecondName(worldManager.crafts[i].finalItem).icon, typeof(Sprite), true, GUILayout.Height(spriteSize / 2), GUILayout.Width(spriteSize / 2));
                    GUILayout.EndHorizontal();
                    GUILayout.EndHorizontal();
                    

                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            bool exitst = false;
            int id = -1;
            for (int i = 0; i < worldManager.crafts.Count; i++)
            {
                if (worldManager.crafts[i].finalItem == finalItem)
                {
                    id = i;
                    exitst = true;
                    break;
                }
            }
            if (exitst == false)
            {
                if (GUILayout.Button("Reset", EditorStyles.miniButtonLeft, GUILayout.Height(buttonHeight)))
                {

                    itemsToCraft.Clear();
                    finalItem = "";
                }
                if (GUILayout.Button("Create", EditorStyles.miniButtonRight, GUILayout.Height(buttonHeight)))
                {
                    var item = new WorldManager.Craft();
                    item.finalItem = finalItem;
                    item.craftType = craftType.ToString();
                    item.crafts = itemsToCraft;
                    if (insert)
                    {
                        worldManager.crafts.Insert(insertValue, item);
                    }
                    else
                    {
                        worldManager.crafts.Insert(0, item);
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Reset", EditorStyles.miniButtonLeft, GUILayout.Height(buttonHeight)))
                {

                    itemsToCraft.Clear();
                    finalItem = "";
                }
                if (GUILayout.Button("Create", EditorStyles.miniButtonMid, GUILayout.Height(buttonHeight)))
                {
                    var item = new WorldManager.Craft();
                    item.finalItem = finalItem;
                    item.craftType = craftType.ToString();
                    item.crafts = itemsToCraft;
                    if (insert)
                    {
                        worldManager.crafts.Insert(insertValue, item);
                    }
                    else
                    {
                        worldManager.crafts.Insert(0, item);
                    }
                }
                if (GUILayout.Button("Rewrite", EditorStyles.miniButtonRight, GUILayout.Height(buttonHeight)))
                {
                    var item = new WorldManager.Craft();
                    item.finalItem = finalItem;
                    item.craftType = craftType.ToString();
                    item.crafts = itemsToCraft;
                    worldManager.crafts.RemoveAt(id);
                    if (insert)
                    {
                        worldManager.crafts.Insert(insertValue, item);
                    }
                    else
                    {
                        worldManager.crafts.Insert(id, item);
                    }
                }

            }
            GUILayout.EndHorizontal();
        }
        if (tab == 2)
        {
           
            scroll = EditorGUILayout.BeginScrollView(scroll);
           
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            //if (GUILayout.Button("Создать город", EditorStyles.miniButton, GUILayout.Height(buttonHeight)))
            //{
            //    TownConfig file = new TownConfig();

            //    file._name = _name;
            //    file.count = count;
            //    file.NPC_Prototypes = NPC_Prototypes;

            //    AssetDatabase.CreateAsset(file, "Assets/Game Custom Files/Towns/" + _fileName + ".asset");
            //    ItemList itemList = FindObjectOfType<ItemList>();
            //    itemList.AddT(file);
            //}
        }
    }
}
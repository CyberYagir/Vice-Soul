using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildDig : MonoBehaviour
{
    public PlayerUI playerUI;
    PlayerStats playerStats;
    public LineRenderer line;
    public Transform blockPrewiew;
    public Tilemap main, back, decor;
    [Space]
    public WorldManager manager;

    public int mineRange;

    public Vector2Int cursorPos;
    public Vector2Int oldCursorPos;
    public bool dig = false, digTree = false, digHam = false, bowsh = false;
    public float digTime = 0;

    public Sprite stdSprite;

    public List<Sprite> digSprites;

    public SpriteRenderer hands;
    public Transform handTorch;

    public SpriteRenderer digSprite, blockPrewiewSprite, digSpritePlacePreview, debug;
    [Space]
    public float blockTime;
    public WorldManager.Block block;
    public bool isMain;
    [Space]

    public bool started, blockIsPlaced, attacked;

    public IEnumerator BlockBreak, TreeBreak, BlockPlace, MeeleCooldown, HammerBreak, BowCooldown;

    Vector3 mouse;
    public LayerMask mask1;

    public string selectedArrows;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        manager = FindObjectOfType<WorldManager>();
        blockPrewiewSprite = blockPrewiew.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var playerPos = main.CellToWorld(main.WorldToCell(transform.position));
        mouse = main.CellToWorld(main.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        if (mouse.x < playerPos.x + mineRange + 1 && mouse.x > playerPos.x - mineRange - 1)
        {
            if (mouse.y > playerPos.y - mineRange - 1 && mouse.y < playerPos.y + mineRange + 1)
            {
                blockPrewiewSprite.color = new Color(255, 255, 255, blockPrewiewSprite.color.a);
            }
            else
            {
                blockPrewiewSprite.color = new Color(255, 0, 0, blockPrewiewSprite.color.a);
            }
        }
        else
        {

            blockPrewiewSprite.color = new Color(255, 0, 0, blockPrewiewSprite.color.a);
        }


        if (mouse.y > playerPos.y + mineRange)
        {
            mouse = new Vector3(mouse.x, playerPos.y + mineRange, 0);
        }
        if (mouse.y < playerPos.y - mineRange)
        {
            mouse = new Vector3(mouse.x, playerPos.y - mineRange, 0);
        }
        if (mouse.x < playerPos.x - mineRange)
        {
            mouse = new Vector3(playerPos.x - mineRange, mouse.y, 0);
        }
        if (mouse.x > playerPos.x + mineRange)
        {
            mouse = new Vector3(playerPos.x + mineRange, mouse.y, 0);
        }

        blockPrewiew.transform.position = mouse;
        if (oldCursorPos != cursorPos)
        {
            if (BlockBreak != null)
            {
                StopCoroutine(BlockBreak);
                dig = false;
            }
            if (HammerBreak != null)
            {
                StopCoroutine(HammerBreak);
                HammerBreak = null;
                digHam = false;
            }
            started = false;
        }
        oldCursorPos = cursorPos;
        cursorPos = new Vector2Int((int)mouse.x, (int)mouse.y);

        if (cursorPos != oldCursorPos)
        {
            digTime = 0;
            block = null;

        }
        if (dig)
        {
            if (block != null)
            {
                DigWait(block, isMain);

                if (digTime < (float)block.digTime / 3f)
                {
                    digSprite.sprite = digSprites[0];
                }
                else
                if (digTime < (float)block.digTime / 2f)
                {
                    digSprite.sprite = digSprites[1];
                }
                else
                if (digTime < (float)block.digTime)
                {
                    digSprite.sprite = digSprites[2];
                }

                if (isMain == true)
                {
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        started = false;
                        dig = false;
                        digTime = 0;
                    }
                }
                else
                {
                    if (Input.GetKeyUp(KeyCode.Mouse1))
                    {
                        started = false;
                        dig = false;
                        digTime = 0;
                    }
                }
                return;
            }
            else if (digHam == false)
            {
                digSprite.sprite = null;
                digTime = 0;
            }
        }
        else
        {
            if (digHam == false)
            {
                digSprite.sprite = null;
                digTime = 0;
            }
        }


        line.SetPosition(0, transform.position + new Vector3(0, 0, -0.5f));
        line.SetPosition(1, blockPrewiew.transform.position + new Vector3(0.5f, 0.5f, -0.5f));


        if (playerUI.hotBarItems[playerUI.selectedCase] == -1)
        {
            playerStats.localPlayer.digForce = 1;
            if (dig == false && digHam == false)
            {
                digSpritePlacePreview.sprite = null;
            }
            digSpritePlacePreview.transform.localPosition = new Vector2(0, 0);
            //blockPrewiewSprite.transform.localScale = new Vector3(1, 1);
            blockPrewiewSprite.size = new Vector3(1, 1);
            hands.sprite = null;
            handTorch.gameObject.SetActive(false);
        }
        else
        {



            var anim = hands.transform.parent.GetComponent<Animator>();
            var selected = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]];

            //Torch;
            if (selected.secondName == "_Torch_")
            {
                handTorch.gameObject.SetActive(true);
            }
            else
            {
                handTorch.gameObject.SetActive(false);
            }



            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("None"))
                {
                    if (selected.type == Item.itemType.Melee)
                    {
                        if (MeeleCooldown == null)
                        {
                            anim.Play("Hands1");
                        }
                    }
                    else
                    if (selected.type == Item.itemType.Distance)
                    {
                        anim.Play("Hands2");
                    }
                    else
                    if (selected.type == Item.itemType.Use)
                    {
                        if (!playerUI.inventoryWindow.active)
                            anim.Play("Hands");
                    }
                    else
                    {
                        anim.Play("Hands");
                    }
                }
            }
            else if (Input.GetKey(KeyCode.Mouse1))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("None"))
                {
                    if (selected.type == Item.itemType.Dig)
                    {
                        anim.Play("Hands");
                    }
                }
            }
            else
            {
                if (selected.type == Item.itemType.Distance)
                {
                    anim.Play("Hands2");
                }
            }



            if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && (selected.type == Item.itemType.Block || selected.type == Item.itemType.Table))
            {
                if (selected.type == Item.itemType.Block)
                {
                    digSpritePlacePreview.transform.localPosition = new Vector2(0.5f, 0.5f);
                    digSpritePlacePreview.sprite = selected.tilesprite;
                }
                if (selected.type == Item.itemType.Table)
                {
                    digSpritePlacePreview.transform.localPosition = new Vector2(0, 0);
                    digSpritePlacePreview.sprite = selected.icon;
                }
            }
            else
            {
                digSpritePlacePreview.transform.localPosition = new Vector2(0, 0);
                digSpritePlacePreview.sprite = null;
            }
            if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && (selected.type == Item.itemType.Dig ||
                                                                  selected.type == Item.itemType.Axe ||
                                                                  selected.type == Item.itemType.Melee ||
                                                                  selected.type == Item.itemType.Distance ||
                                                                  selected.type == Item.itemType.Hammer ||
                                                                  selected.type == Item.itemType.Use))
            {
                hands.sprite = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].icon;

                if (selected.type == Item.itemType.Dig || selected.type == Item.itemType.Axe)
                {
                    playerStats.localPlayer.digForce = selected.digLevel;
                }
                hands.transform.parent.localScale = new Vector2(selected.floatSize.x,
                                                                   selected.floatSize.y);
            }
            else
            {
                hands.sprite = null;
            }



            //blockPrewiewSprite.transform.localScale = (Vector2)playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].size;
            blockPrewiewSprite.size = (Vector2)playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].size;
            digSprite.size = (Vector2)playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].size;
        }



        //Прирывание если мошь на интерфейсе.
        if (playerUI.hotBarItemsScripts.FindAll(x => x.GetComponent<HotBarMouseClick>().over == true).Count != 0) return;
        if (FindObjectsOfType<WindowOverCheck>().ToList().FindAll(x => x.over == true && x.gameObject.active == true).Count != 0) return;
        if (playerUI.chest5x5UI.GetComponent<ChestUI>().over == true && playerUI.chest5x5UI.GetComponent<ChestUI>().gameObject.active == true) return;


        //


        if (playerUI.hotBarItems[playerUI.selectedCase] == -1 || playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Dig)
        {
            blockPrewiewSprite.sprite = stdSprite;
            if ((HitTag() == "None" || HitTag() == "Entity"))
            {
                if (HitGameObject() != null)
                {
                    if (HitGameObject().TryGetComponent(out Entity ent) == false)
                    {
                        if (Input.GetKey(KeyCode.Mouse0))
                        {
                            var m = main.WorldToCell(blockPrewiew.position);
                            var p = manager.GetBlock(main.GetTile(m));
                            if (main.GetTile(m) != null && p != null &&
                                (HitGameObject(new Vector3(0, 1, 0)) == null || (HitGameObject(new Vector3(0, 1, 0)).tag == "Player" ||
                                (HitGameObject(new Vector3(0, 1, 0)) != null && HitGameObject(new Vector3(0, 1, 0)).tag != "Tree" && HitGameObject(new Vector3(0, 1, 0)).gameObject.layer != 8))))
                            {
                                if (started == false)
                                {
                                    dig = true;
                                    block = p;
                                    isMain = true;
                                    //BlockBreak = Dig(p, true);
                                    //StartCoroutine(BlockBreak);
                                    started = true;
                                }
                            }
                        }
                        if (Input.GetKeyUp(KeyCode.Mouse0))
                        {
                            started = false;
                            dig = false;
                            digTime = 0;
                        }
                    }
                    else
                    {
                        if (ent.GetComponent<Chest>() != null)
                        {
                            if (ent.GetComponent<Chest>().itemsIn.Count == 0)
                                Hammer();
                        }
                        else
                        {
                            Hammer();
                        }
                    }

                    if (dig == false && digHam == false)
                    {
                        digSprite.sprite = null;
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        var m = main.WorldToCell(blockPrewiew.position);
                        var p = manager.GetBlock(main.GetTile(m));
                        if (p == null)
                        {
                            var g = manager.GetBlock(back.GetTile(m));
                            if (g != null)
                            {
                                if (started == false)
                                {
                                    dig = true;
                                    block = g;
                                    isMain = false;
                                    //BlockBreak = Dig(g, false);
                                    //StartCoroutine(BlockBreak);
                                    started = true;
                                }
                            }
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.Mouse1))
                    {
                        started = false;
                        dig = false;
                        digTime = 0;
                    }

                    if (dig == false && digHam == false)
                    {
                        digSprite.sprite = null;
                    }
                }
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Block)
        {
            if (playerUI.inventoryWindow.active == false)
            {
                if (!blockIsPlaced)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (HitTag() == "None")
                        {
                            var m = main.WorldToCell(blockPrewiew.position);
                            var p = main.GetTile(m);
                            if (p == null && (main.GetTile(m + new Vector3Int(1, 0, 0)) != null || main.GetTile(m + new Vector3Int(-1, 0, 0)) != null || main.GetTile(m + new Vector3Int(0, 1, 0)) != null || main.GetTile(m + new Vector3Int(0, -1, 0)) != null || back.GetTile(m)))
                            {
                                if (playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].value > 0)
                                {
                                    main.SetTile(m, playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].tile);
                                    decor.SetTile(m, null);
                                    playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName));

                                    //playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].value -= 1;
                                    //if (playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].value == 0)
                                    //{
                                    //    playerStats.localPlayer.inventory.RemoveAt(playerUI.hotBarItems[playerUI.selectedCase]);
                                    //    playerUI.hotBarItems[playerUI.selectedCase] = -1;
                                    //    return;
                                    //}
                                    playerUI.UpdateInventory(true);
                                    playerUI.UpdateHotbar();
                                    blockIsPlaced = true;
                                    StartCoroutine(SetBlock());
                                }
                            }
                        }
                    }
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        var m = back.WorldToCell(blockPrewiew.position);
                        var p = back.GetTile(m);
                        if (p == null && (main.GetTile(m + new Vector3Int(1, 0, 0)) != null || main.GetTile(m + new Vector3Int(-1, 0, 0)) != null || main.GetTile(m + new Vector3Int(0, 1, 0)) != null || main.GetTile(m + new Vector3Int(0, -1, 0)) != null || back.GetTile(m + new Vector3Int(1, 0, 0)) != null || back.GetTile(m + new Vector3Int(-1, 0, 0)) != null || back.GetTile(m + new Vector3Int(0, 1, 0)) != null || back.GetTile(m + new Vector3Int(0, -1, 0)) != null))
                        {
                            if (playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].value > 0)
                            {
                                back.SetTile(m, playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].tile);
                                playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].value -= 1;
                                if (playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].value == 0)
                                {
                                    playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]] = new Item();
                                    playerUI.hotBarItems[playerUI.selectedCase] = -1;
                                }
                                playerUI.UpdateInventory(true);
                                playerUI.UpdateHotbar();
                            }
                        }
                        blockIsPlaced = true;
                        StartCoroutine(SetBlock());
                    }
                }
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] == -1 || playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Axe)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (HitTag(blockPrewiew) == "Tree")
                {
                    if (digTree == false && TreeBreak == null)
                    {
                        if (HitGameObject() != null)
                        {
                            if (HitGameObject().GetComponent<Tree>() != null)
                            {
                                TreeBreak = TreeDig(HitGameObject().GetComponent<Tree>());
                                StartCoroutine(TreeBreak);
                                digTree = true;
                            }
                        }
                    }
                }
                else
                {
                    digTree = false;
                    if (TreeBreak != null)
                    {
                        StopCoroutine(TreeBreak);
                    }
                    TreeBreak = null;
                }
            }
            else
            {
                digTree = false;
                if (TreeBreak != null)
                {
                    StopCoroutine(TreeBreak);
                }
                TreeBreak = null;
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Table)
        {
            if (playerUI.inventoryWindow.active == false)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (HitTag() == "None" || HitTag() == "Player")
                    {
                        if ((HitGameObject(true) == null || HitGameObject(true).tag == "Player") && HitAround(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].size) == true)
                        {
                            if (blockIsPlaced == false)
                            {
                                bool empty = true;
                                int groundBlocks = 0;
                                for (int x = 0; x < playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].size.x; x++)
                                {
                                    for (int y = 0; y < playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].size.y; y++)
                                    {
                                        var m = main.WorldToCell(blockPrewiew.position + new Vector3(x, y));
                                        if (main.GetTile(m) != null)
                                        {
                                            empty = false;
                                        }
                                    }
                                }
                                for (int x = 0; x < playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].size.x; x++)
                                {
                                    var m = main.WorldToCell(blockPrewiew.position + new Vector3(x, -1));
                                    if (main.GetTile(m) != null)
                                    {
                                        groundBlocks++;
                                    }
                                }
                                if (empty == true)
                                {
                                    Debug.DrawRay(blockPrewiew.position + new Vector3(0.5f, 0.5f, -5f), Vector3.forward);
                                    var m = main.WorldToCell(blockPrewiew.position + new Vector3(0.5f, 0.5f));
                                    var p = main.GetTile(m);
                                    var item = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]];

                                    if (p == null && ((item.onGround == true && groundBlocks == item.size.x) || ((item.onGround == false && (back.GetTile(m) != null) || groundBlocks == item.size.x))))
                                    {
                                        if (item.value > 0)
                                        {
                                            GameObject g = Instantiate(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].prefab, blockPrewiew.position, Quaternion.identity);
                                            //main.SetTile(m, playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].tile);
                                            if (g.GetComponent<Entity>() != null)
                                                g.GetComponent<Entity>().chunk = FindObjectOfType<WorldGenerator>().GetChunkByPos(g.transform);
                                            playerUI.RemoveItem(manager.GetItemBySecondName(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName));
                                            playerUI.UpdateInventory(true);
                                            blockIsPlaced = true;
                                            StartCoroutine(SetBlock());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Melee)
        {
            if (attacked == false && MeeleCooldown == null)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    print("Attack");
                    var selected = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]];

                    MeeleCooldown = MeeleWait();
                    StartCoroutine(MeeleCooldown);
                    attacked = true;
                }
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Hammer)
        {
            Hammer();
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Helmet)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetComponent<Armor>().helmet != "")
                {
                    playerUI.AddItem(manager.GetItemBySecondName(GetComponent<Armor>().helmet));
                }
                GetComponent<Armor>().helmet = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName;
                playerUI.RemoveItem(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]]);
                return;
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Body)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetComponent<Armor>().body != "")
                {
                    playerUI.AddItem(manager.GetItemBySecondName(GetComponent<Armor>().body));
                }
                GetComponent<Armor>().body = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName;
                playerUI.RemoveItem(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]]);
                return;
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Backpack)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetComponent<Armor>().backpack != "")
                {
                    print(GetComponent<Armor>().backpack);
                    playerUI.AddItem(manager.GetItemBySecondName(GetComponent<Armor>().backpack));
                }
                GetComponent<Armor>().backpack = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName;
                playerUI.RemoveItem(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]]);
                return;
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Foots)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetComponent<Armor>().foots != "")
                {
                    print(GetComponent<Armor>().foots);
                    playerUI.AddItem(manager.GetItemBySecondName(GetComponent<Armor>().foots));
                }
                GetComponent<Armor>().foots = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName;
                playerUI.RemoveItem(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]]);
                return;
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Hook)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetComponent<Armor>().hook != "")
                {
                    print(GetComponent<Armor>().hook);
                    playerUI.AddItem(manager.GetItemBySecondName(GetComponent<Armor>().hook));
                }
                GetComponent<Armor>().hook = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName;
                playerUI.RemoveItem(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]]);
                return;
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Hands)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (GetComponent<Armor>().hands != "")
                {
                    playerUI.AddItem(manager.GetItemBySecondName(GetComponent<Armor>().hands));
                }
                GetComponent<Armor>().hands = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].secondName;
                playerUI.RemoveItem(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]]);
                return;
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Distance)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (bowsh == false && BowCooldown == null)
                {
                    if (playerUI.inventoryWindow.active == false)
                    {
                        if (playerUI.FindItemInInventoryByName(selectedArrows) == false)
                        {
                            selectedArrows = "";
                        }
                        string bullet = selectedArrows;

                        if (bullet == "")
                        {

                            if (playerUI.GetItemInInventoryByData("bullet") != null)
                            {
                                bullet = playerUI.GetItemInInventoryByData("bullet").secondName;
                                if (bullet == "") return;

                            }
                        }
                        if (playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].data.ToLower().Contains(bullet.ToLower()))
                        {
                            GameObject arr = Instantiate(manager.GetItemBySecondName(bullet).prefab, hands.transform.position, Quaternion.identity);
                            arr.transform.right = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)arr.transform.position;
                            if (transform.localScale.x < 0)
                            {
                                arr.transform.localScale = new Vector3(arr.transform.localScale.x, arr.transform.localScale.y, arr.transform.localScale.z);
                            }
                            if (transform.localScale.x > 0)
                            {
                                arr.transform.localScale = new Vector3(arr.transform.localScale.x, arr.transform.localScale.y, arr.transform.localScale.z);
                            }
                            arr.GetComponent<Arrow>().damage += playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].damage;
                            if (!manager.GetItemBySecondName(bullet).data.Contains("infinity"))
                            {
                                playerUI.RemoveItem(manager.GetItemBySecondName(bullet));
                            }
                            BowCooldown = BowWait(playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]]);
                            bowsh = true;
                            StartCoroutine(BowCooldown);
                        }
                    }
                }
            }
        }
        if (playerUI.hotBarItems[playerUI.selectedCase] != -1 && playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]].type == Item.itemType.Use)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!playerUI.inventoryWindow.active)
                {
                    var sl = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]];
                    if (sl.secondName.ToLower() == "_bondage_" || sl.secondName.ToLower() == "_redbottle_" || sl.secondName.ToLower() == "_rawmeat_" || sl.secondName.ToLower() == "_cookmeat_" || sl.secondName.ToLower() == "_cactuspart_")
                    {
                        if (playerStats.localPlayer.health < playerStats.localPlayer.dopHealth + playerStats.localPlayer.stdHealth)
                        {
                            playerStats.localPlayer.health += int.Parse(sl.data);
                            playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(sl.secondName));

                            if (playerStats.localPlayer.health > playerStats.localPlayer.dopHealth + playerStats.localPlayer.stdHealth)
                            {
                                playerStats.localPlayer.health = playerStats.localPlayer.dopHealth + playerStats.localPlayer.stdHealth;
                            }
                        }
                    }
                    if (sl.secondName.ToLower() == "_speedbottle_")
                    {
                        var eff = GetComponent<Effects>();
                        if (eff.effects.Find(x => x.name == eff.AllEffects[3].name) == null)
                        {
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            eff.AddEffect(3);
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(sl.secondName));
                        }
                    }
                    if (sl.secondName.ToLower() == "_jumpbottle_")
                    {
                        var eff = GetComponent<Effects>();
                        if (eff.effects.Find(x => x.name == eff.AllEffects[4].name) == null)
                        {
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            eff.AddEffect(4);
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(sl.secondName));
                        }
                    }
                    if (sl.secondName.ToLower() == "_regbottle_")
                    {
                        var eff = GetComponent<Effects>();
                        if (eff.effects.Find(x => x.name == eff.AllEffects[5].name) == null)
                        {
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            eff.AddEffect(5);
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(sl.secondName));
                        }
                    }
                    if (sl.secondName.ToLower() == "_backbottle_")
                    {
                        var eff = GetComponent<Effects>();
                        if (eff.effects.Find(x => x.name == eff.AllEffects[2].name) == null)
                        {
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            eff.AddEffect(2);
                            transform.position = GetComponent<PlayerStart>().playerStart;
                            Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                            playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(sl.secondName));
                        }
                    }

                    if (sl.secondName.ToLower() == "_saphirewand_")
                    {
                        var eff = GetComponent<Effects>();
                        if (eff.effects.Find(x => x.name == eff.AllEffects[2].name) == null)
                        {


                            if (main.GetTile(main.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition))) == null)
                            {
                                if (main.GetTile(main.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 1, 0))) == null)
                                {
                                    Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                                    eff.AddEffect(2);
                                    eff.effects[eff.effects.Count - 1].time = 8;
                                    playerStats.TakeDamage(10);
                                    transform.position = main.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(1f, 2f, 0));
                                    Instantiate(playerStats.localPlayer.deadParticles, transform.position, Quaternion.identity).SetActive(true);
                                }
                            }

                        }
                    }

                    if (sl.secondName.ToLower() == "_heart_")
                    {
                        if (playerStats.localPlayer.dopHealth < 100)
                        {
                            playerStats.localPlayer.dopHealth += int.Parse(sl.data);
                            playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(sl.secondName));

                            if (playerStats.localPlayer.health > playerStats.localPlayer.dopHealth + playerStats.localPlayer.stdHealth)
                            {
                                playerStats.localPlayer.health = playerStats.localPlayer.dopHealth + playerStats.localPlayer.stdHealth;
                            }
                        }
                    }
                    if (sl.secondName.ToLower() == "_handbomb_" || sl.secondName.ToLower() == "_cactusoil_")
                    {
                        GameObject bomb = Instantiate(sl.prefab, hands.transform.position, Quaternion.identity);
                        Vector2 dir = blockPrewiew.transform.position - transform.position;
                        bomb.GetComponent<Rigidbody2D>().AddForce(dir * 1000 * Time.deltaTime);

                        playerUI.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName(sl.secondName));
                    }
                }
            }
        }


    }
    IEnumerator BowWait(Item item)
    {
        yield return new WaitForSeconds(item.coolDown);
        bowsh = false;
        BowCooldown = null;
    }

    public void Hammer()
    {
        blockPrewiewSprite.sprite = stdSprite;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (HitTag((Vector2)blockPrewiew.position + new Vector2(0.5f, 0.5f)) == "Entity")
            {
                if (HitGameObject() != null)
                {
                    if (HitGameObject().gameObject.layer != 0)
                    {
                        if (HammerBreak == null)
                        {
                            if (HitGameObject().GetComponent<Entity>().digLvl <= playerStats.localPlayer.digForce)
                            {
                                if (started == false)
                                {
                                    digHam = true;
                                    HammerBreak = Hammer(HitGameObject().GetComponent<Entity>());
                                    StartCoroutine(HammerBreak);
                                    started = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (HammerBreak != null)
            {
                StopCoroutine(HammerBreak);
                HammerBreak = null;
            }
            digHam = false;
            started = false;
        }
    }

    public string HitTag()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.transform.tag == "Untagged")
            {
                return "None";
            }
            return hit.transform.tag;
        }
        return "None";
    }
    public string HitTag(Transform _)
    {
        RaycastHit2D hit = Physics2D.Raycast(_.position, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.transform.tag == "Untagged")
            {
                return "None";
            }
            return hit.transform.tag;
        }
        return "None";
    }
    public string HitTag(Vector2 _)
    {
        RaycastHit2D hit = Physics2D.Raycast(_, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.transform.tag == "Untagged")
            {
                return "None";
            }
            return hit.transform.tag;
        }
        return "None";
    }
    public GameObject HitGameObject()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            return hit.transform.gameObject;
        }
        return null;
    }
    public GameObject HitGameObject(Vector3 offcet)
    {
        var playerPos = main.CellToWorld(main.WorldToCell(transform.position));
        var m = mouse + new Vector3(0.5f, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(m + offcet, Vector2.zero, Mathf.Infinity, mask1);
        if (hit.collider != null)
        {
            debug.transform.position = m + offcet;
            return hit.transform.gameObject;
        }
        return null;
    }
    public GameObject HitGameObject(bool round)
    {
        var m = mouse + new Vector3(0.5f, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(m + new Vector3(0, 0, 0), Vector2.zero);

        if (hit.collider != null)
        {
            print("Hit round " + hit.collider.name);

            return hit.transform.gameObject;
        }
        return null;
    }
    public bool HitAround(Vector2Int size)
    {

        var playerPos = main.CellToWorld(main.WorldToCell(transform.position));
        var m = mouse + new Vector3(0.5f, 0.5f);
        if (m.y > playerPos.y + mineRange)
        {
            m = new Vector3(m.x, playerPos.y + mineRange, 0);
        }
        if (m.y < playerPos.y - mineRange)
        {
            m = new Vector3(m.x, playerPos.y - mineRange, 0);
        }
        if (m.x < playerPos.x - mineRange)
        {
            m = new Vector3(playerPos.x - mineRange, m.y, 0);
        }
        if (m.x > playerPos.x + mineRange)
        {
            m = new Vector3(playerPos.x + mineRange, m.y, 0);
        }
        bool allOk = true;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                RaycastHit2D _hit = Physics2D.Raycast(m + new Vector3(x, y, 0), Vector2.zero);
                if (_hit.collider != null && _hit.transform.tag != "Player")
                {
                    print(_hit.transform.name);
                    allOk = false;
                }
            }
        }
        return allOk;
    }




    IEnumerator MeeleWait()
    {
        var selected = playerStats.localPlayer.inventory[playerUI.hotBarItems[playerUI.selectedCase]];
        GameObject sprite = hands.transform.GetComponent<SpriteRenderer>().gameObject;
        var mobs = FindObjectsOfType<MobInfo>();
        for (int i = 0; i < mobs.Length; i++)
        {
            if ((mobs[i].transform.position.x < transform.position.x && transform.localScale.x < 0) || (mobs[i].transform.position.x > transform.position.x && transform.localScale.x > 0))
            {
                var dist = Vector2.Distance(hands.transform.position, mobs[i].transform.position);
                Vector2 heading = mobs[i].transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(hands.transform.position, heading, selected.distance);
                Debug.DrawRay(hands.transform.position, heading, Color.white, 5);
                if (hit.collider != null && hit.transform.tag == "Mob")
                {
                    if (Vector2.Distance(hit.point, hands.transform.position) <= selected.distance)
                    {
                        var rb = hit.transform.GetComponent<Rigidbody2D>();

                        hit.transform.GetComponent<MobStats>().hp -= (float)selected.damage / (float)10;
                        if (rb != null)
                        {
                            if (transform.localScale.x > 0)
                            {
                                rb.AddRelativeForce(Vector3.right * selected.meeleBackForce * Time.deltaTime, ForceMode2D.Impulse);
                            }
                            if (transform.localScale.x < 0)
                            {
                                rb.AddRelativeForce(Vector3.left * selected.meeleBackForce * Time.deltaTime, ForceMode2D.Impulse);
                            }
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(selected.coolDown);
        StopCoroutine(MeeleCooldown);
        MeeleCooldown = null;
        attacked = false;
    }

    void AttackMeele(Item selected, GameObject sprite)
    {
        if (playerStats.transform.localScale.x > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(hands.transform.position, hands.transform.right, selected.distance);
            Debug.DrawRay(hands.transform.position, hands.transform.right, Color.white, 5);
            if (hit.collider != null)
            {
                if (hit.transform.tag == "Mob")
                {
                    var rb = hit.transform.GetComponent<Rigidbody2D>();
                    hit.transform.GetComponent<MobStats>().hp -= (float)selected.damage / (float)10;
                    if (transform.localScale.x > 0)
                    {
                        rb.AddRelativeForce(Vector3.right * selected.meeleBackForce * Time.deltaTime, ForceMode2D.Impulse);
                    }
                    if (transform.localScale.x < 0)
                    {
                        rb.AddRelativeForce(Vector3.left * selected.meeleBackForce * Time.deltaTime, ForceMode2D.Impulse);
                    }
                }
            }
        }
        if (playerStats.transform.localScale.x < 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(sprite.transform.position, -sprite.transform.right, selected.distance);
            Debug.DrawRay(hands.transform.position, hands.transform.right, Color.white, 5);
            if (hit.collider != null)
            {
                if (hit.transform.tag == "Mob")
                {
                    hit.transform.GetComponent<MobStats>().hp -= (float)selected.damage / (float)10;
                }
            }
        }
    }

    IEnumerator SetBlock()
    {
        yield return new WaitForSeconds(blockTime);
        blockIsPlaced = false;
    }

    IEnumerator TreeDig(Tree tree)
    {
        bool secondDig = true;
        while (secondDig == true)
        {
            //print(tree.hp);
            yield return new WaitForSeconds(0.1f);
            tree.speed = (tree.starthp / tree.hp) * playerStats.localPlayer.digForce;
            tree.hp -= (tree.starthp / tree.hp) * playerStats.localPlayer.digForce * Time.deltaTime;
            if (digTree == false)
            {
                secondDig = false;
                break;
            }
            if (tree.hp <= 0)
            {
                started = false;
                secondDig = false;
                break;
            }
        }
    }

    IEnumerator Hammer(Entity block)
    {
        bool secondDig = digHam;
        //print(((float)block.digTime / (float)playerStats.localPlayer.digForce));
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(((float)block.digTime / (float)playerStats.localPlayer.digForce) / 20);
            if (digHam == false)
            {
                secondDig = false;
            }
            if (i <= 2)
            {
                digSprite.sprite = null;
            }
            if (i > 2)
            {
                digSprite.sprite = digSprites[0];
            }
            if (i > 10)
            {
                digSprite.sprite = digSprites[1];
            }
            if (i > 18)
            {
                digSprite.sprite = digSprites[2];
            }
        }
        print(secondDig);
        digSprite.sprite = null;
        if (0 <= playerStats.localPlayer.digForce)
        {
            if (secondDig == true)
            {
                playerStats.GetComponent<PlayerUI>().AddItem(manager.GetItemBySecondName(block.entityID));
                Destroy(block.gameObject);

                started = false;
                digHam = false;
                HammerBreak = null;
                yield break;
            }
            else
            {
                started = false;
                digHam = false;
                HammerBreak = null;
                yield break;
            }
        }
        started = false;
        digHam = false;
        HammerBreak = null;
        yield break;
    }

    IEnumerator Dig(WorldManager.Block block, bool IsMain)
    {
        bool secondDig = dig;
        //print(((float)block.digTime / (float)playerStats.localPlayer.digForce) / 20);
        for (int i = 0; i < 20; i++)
        {

            yield return new WaitForSeconds(((float)block.digTime / (float)playerStats.localPlayer.digForce) / 20);
            if (dig == false)
            {
                secondDig = false;
            }
            if (i <= 2)
            {
                digSprite.sprite = null;
            }
            if (i > 2)
            {
                digSprite.sprite = digSprites[0];
            }
            if (i > 10)
            {
                digSprite.sprite = digSprites[1];
            }
            if (i > 18)
            {
                digSprite.sprite = digSprites[2];
            }
        }
        digSprite.sprite = null;
        if (block.force <= playerStats.localPlayer.digForce)
        {
            if (secondDig == true)
            {
                if (IsMain == true)
                {
                    var m = main.WorldToCell(blockPrewiew.position);
                    var p = manager.GetBlock(main.GetTile(m));
                    if (p != null)
                    {
                        if (manager.GetTileID(main.GetTile(m)) == 2 || manager.GetTileID(main.GetTile(m)) == 19)
                        {
                            decor.SetTile(m + new Vector3Int(0, 1, 0), null);
                        }
                        if (manager.GetTileID(main.GetTile(m)) == 14)
                        {
                            var tile = main.GetTile(m + new Vector3Int(0, 1, 0));
                            if (tile == null)
                            {
                                decor.SetTile(m + new Vector3Int(0, 1, 0), null);
                            }
                        }
                        main.SetTile(m, null);
                        decor.SetTile(m, null);
                        for (int d = 0; d < p.drop.Count; d++)
                        {
                            GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(p.drop[d]));
                        }
                        for (int d = 0; d < p.randomDrop.Count; d++)
                        {
                            if (Random.Range(0, 1) == 1)
                            {
                                GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(p.randomDrop[d]));
                            }
                        }
                    }
                    else
                    {
                        started = false;
                        dig = false;
                        yield break;
                    }
                }
                else
                {
                    var m = main.WorldToCell(blockPrewiew.position);
                    var p = main.GetTile(m);
                    if (p == null)
                    {
                        print("Mouse1");
                        if (back.GetTile(m) != null)
                        {
                            var g = manager.GetBlock(back.GetTile(m));
                            if (g != null)
                            {
                                back.SetTile(m, null);
                                for (int d = 0; d < g.drop.Count; d++)
                                {
                                    GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(g.drop[d]));
                                }
                                for (int d = 0; d < g.randomDrop.Count; d++)
                                {
                                    if (Random.Range(0, 1) == 1)
                                    {
                                        GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(g.randomDrop[d]));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        started = false;
                        dig = false;
                        yield break;
                    }
                }
            }
        }
        started = false;
        dig = false;
        yield break;
    }


    void DigWait(WorldManager.Block block, bool IsMain)
    {
        if (digTime <= block.digTime)
        {
            digTime += (((float)block.digTime * (float)playerStats.localPlayer.digForce)) * Time.deltaTime;
            return;

        }
        else
        {
            digSprite.sprite = null;
            print(block.force + "<=" + playerStats.localPlayer.digForce);
            if (block.force <= playerStats.localPlayer.digForce)
            {
                if (IsMain == true)
                {
                    var m = main.WorldToCell(blockPrewiew.position);
                    var p = manager.GetBlock(main.GetTile(m));
                    if (p != null)
                    {
                        if (manager.GetTileID(main.GetTile(m)) == 2 || manager.GetTileID(main.GetTile(m)) == 19)
                        {
                            decor.SetTile(m + new Vector3Int(0, 1, 0), null);
                        }
                        if (manager.GetTileID(main.GetTile(m)) == 14)
                        {
                            var tile = main.GetTile(m + new Vector3Int(0, 1, 0));
                            if (tile == null)
                            {
                                decor.SetTile(m + new Vector3Int(0, 1, 0), null);
                            }
                        }
                        main.SetTile(m, null);
                        decor.SetTile(m, null);
                        for (int d = 0; d < p.drop.Count; d++)
                        {
                            GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(p.drop[d]));
                        }
                        for (int d = 0; d < p.randomDrop.Count; d++)
                        {
                            if (Random.Range(0, 1) == 1)
                            {
                                GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(p.randomDrop[d]));
                            }
                        }
                    }
                    else
                    {
                        digTime = 0;
                        started = false;
                        dig = false;
                        return;
                    }
                }
                else
                {
                    var m = main.WorldToCell(blockPrewiew.position);
                    var p = main.GetTile(m);
                    if (p == null)
                    {
                        print("Mouse1");
                        if (back.GetTile(m) != null)
                        {
                            var g = manager.GetBlock(back.GetTile(m));
                            if (g != null)
                            {
                                back.SetTile(m, null);
                                for (int d = 0; d < g.drop.Count; d++)
                                {
                                    GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(g.drop[d]));
                                }
                                for (int d = 0; d < g.randomDrop.Count; d++)
                                {
                                    if (Random.Range(0, 1) == 1)
                                    {
                                        GetComponent<PlayerUI>().AddItem(FindObjectOfType<WorldManager>().GetItemBySecondName(g.randomDrop[d]));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        digTime = 0;
                        started = false;
                        dig = false;
                        return;
                    }
                }
            }
        }
        digTime = 0;
        started = false;
        dig = false;
        return;
    }
}

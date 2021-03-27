using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Armor : MonoBehaviour
{
    public Sprite helmetStd, bodyStd, handsStd, footStd;
    public SpriteRenderer partHelmet, partBody, partHand1, partHand2, partBackPack, partFoot1, partFoot2;


    public string helmet = "", body = "", hands = "", backpack = "", foots= "", hook = "";

    public WorldManager manager;
    public PlayerStats stats;
    public ShadowTileMap shadow;
    public Light light;

    [Header("UI")]
    public Button helmetButton;
    public Button bodyButton;
    public Button handsButton;
    public Button backpackButton;
    public Button footButton;
    public Button hookButton;
    public Hook hookS;
    public GameObject playerWindow;
    public Camera playerCamera;
    private void Update()
    {
        playerCamera.gameObject.active = playerWindow.active;
        stats.localPlayer.armor = stats.localPlayer.stdArmor;
        stats.localPlayer.playerSpeed = stats.localPlayer.stdPlayerSpeed;
        stats.localPlayer.playerJump = stats.localPlayer.stdPlayerJump;
        stats.localPlayer.lightRange = 2;
        var hl = manager.GetItemBySecondName(helmet, true);
        var bd = manager.GetItemBySecondName(body, true);
        var hd = manager.GetItemBySecondName(hands, true);
        var bp = manager.GetItemBySecondName(backpack);
        var ft = manager.GetItemBySecondName(foots);
        var hk = manager.GetItemBySecondName(hook);
        bool gravChange = false;
        if (hl != null)
        {
            partHelmet.sprite = hl.icon;
            stats.localPlayer.armor += hl.armor;
            helmetButton.gameObject.SetActive(true);
            helmetButton.GetComponent<Image>().sprite = hl.icon;
        }
        else
        {
            helmetButton.gameObject.SetActive(false);
            partHelmet.sprite = helmetStd;
        }
        if (bd != null)
        {
            partBody.sprite = bd.icon;
            stats.localPlayer.armor += bd.armor;
            bodyButton.gameObject.SetActive(true);
            bodyButton.GetComponent<Image>().sprite = bd.icon;
        }
        else
        {
            bodyButton.gameObject.SetActive(false);
            partBody.sprite = bodyStd;
        }
        if (hd != null)
        {
            partHand1.sprite = hd.icon;
            partHand2.sprite = hd.icon;
            stats.localPlayer.armor += hd.armor;

            handsButton.gameObject.SetActive(true);
            handsButton.GetComponent<Image>().sprite = hd.icon;
        }
        else
        {
            handsButton.gameObject.SetActive(false);
            partHand1.sprite = handsStd;
            partHand2.sprite = handsStd;
        }
        if (bp != null)
        {
            int biome = -1;
            partBackPack.sprite = bp.icon;
            stats.localPlayer.armor += bp.armor;
            var sp = bp.data.Split(';').ToArray();
            for (int i = 0; i < sp.Length; i++)
            {
                var sp1 = sp[i].Split(':');
                if (sp1.Contains("biome"))
                {
                    biome = int.Parse(sp1[1]);
                }
            }

            for (int i = 0; i < sp.Length; i++)
            {
                var sp1 = sp[i].Split(':');
                if (biome == -1)
                {
                    if (sp1.Contains("light"))
                    {
                        light.range = float.Parse(sp1[1]);
                    }
                    if (sp1.Contains("fly"))
                    {
                        stats.localPlayer.controller.rb.gravityScale = int.Parse(sp1[1]);
                        gravChange = true;
                    }
                    if (sp1.Contains("speed"))
                    {
                        stats.localPlayer.playerSpeed += int.Parse(sp1[1]);
                    }
                    if (sp1.Contains("shadow"))
                    {
                        stats.localPlayer.lightRange += int.Parse(sp1[1]);
                    }
                }
                else
                {
                    if (biome == FindObjectOfType<WorldGenerator>().playerChunk.biom)
                    {
                        print(biome);
                        if (sp1.Contains("light"))
                        {
                            light.range = float.Parse(sp1[1]);
                        }
                        if (sp1.Contains("fly"))
                        {
                            stats.localPlayer.controller.rb.gravityScale = int.Parse(sp1[1]);
                            gravChange = true;
                        }
                        if (sp1.Contains("speed"))
                        {
                            stats.localPlayer.playerSpeed += int.Parse(sp1[1]);
                        }
                    }
                    else
                    {
                        light.range = 0;
                    }
                }
            }


            
            backpackButton.gameObject.SetActive(true);
            backpackButton.GetComponent<Image>().sprite = bp.icon;
        }
        else
        {
            if (gravChange == false)
            {
                stats.localPlayer.controller.rb.gravityScale = 5;
            }
            light.range = 0;
            backpackButton.gameObject.SetActive(false);
            partBackPack.sprite = null;
        }
        if (ft != null)
        {
            partFoot1.sprite = ft.icon;
            partFoot2.sprite = ft.icon;
            stats.localPlayer.armor += ft.armor;
            var sp = ft.data.Split(';').ToArray();
            for (int i = 0; i < sp.Length; i++)
            {
                var sp1 = sp[i].Split(':');

                if (sp1.Contains("light"))
                {
                    light.range = float.Parse(sp1[1]);
                }
                if (sp1.Contains("fly"))
                {
                    stats.localPlayer.controller.rb.gravityScale = int.Parse(sp1[1]);
                }
                if (sp1.Contains("speed"))
                {
                    stats.localPlayer.playerSpeed += int.Parse(sp1[1]);
                }
                if (sp1.Contains("jump"))
                {
                    stats.localPlayer.playerJump += int.Parse(sp1[1]);
                }
            }
            footButton.gameObject.SetActive(true);
            footButton.GetComponent<Image>().sprite = ft.icon;
        }
        else
        {
            light.range = 0;
            if (gravChange == false)
            {
                stats.localPlayer.controller.rb.gravityScale = 5;
            }
            footButton.gameObject.SetActive(false);
            partFoot1.sprite = footStd;
            partFoot2.sprite = footStd;
        }

        if (hk != null)
        {
            hookButton.gameObject.SetActive(true);
            hookButton.GetComponent<Image>().sprite = hk.icon;

        }
        else
        {
            hookButton.gameObject.SetActive(false);
        }
        hookS.enabled = hk != null;
    }

    public void UnEquipHelmet()
    {
        stats.GetComponent<PlayerUI>().AddItem(manager.GetItemBySecondName(helmet, true));
        helmet = "";
    }
    public void UnEquipBody()
    {
        stats.GetComponent<PlayerUI>().AddItem(manager.GetItemBySecondName(body,true));
        body = "";
    }
    public void UnEquipHands()
    {
        stats.GetComponent<PlayerUI>().AddItem(manager.GetItemBySecondName(hands,true));
        hands = "";
    }
    public void UnEquipBackpack()
    {
        stats.GetComponent<PlayerUI>().AddItem(manager.GetItemBySecondName(backpack,true));
        backpack = "";
    }
    public void UnEquipFoots()
    {
        stats.GetComponent<PlayerUI>().AddItem(manager.GetItemBySecondName(foots, true));
        foots = "";
    }

    public void UnEquipHook()
    {
        if (hookS.hooked == false)
        {
            stats.GetComponent<PlayerUI>().AddItem(manager.GetItemBySecondName(hook, true));
            hook = "";
        }
    }

}

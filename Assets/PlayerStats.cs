using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerStats : MonoBehaviour
{
    public Player localPlayer = new Player();


    private void Start()
    {
        localPlayer.PlayerStart(transform);
        GetComponent<PlayerUI>().HealthUpdate();
    }
    public void TakeDamage(float damage)
    {
        localPlayer.health -= (int)(damage - ((damage / 100) * localPlayer.armor));
    }
    private void Update()
    {
        if (localPlayer.canDeath)
        {
            GetComponent<PlayerDeath>().enabled = true;
        }
        else
        {
            GetComponent<PlayerDeath>().enabled = false;
        }
        if (localPlayer.health > localPlayer.stdHealth + localPlayer.dopHealth)
        {
            localPlayer.health = localPlayer.stdHealth + localPlayer.dopHealth;
        }
        if (localPlayer.oldhp != localPlayer.health)
        {
            GetComponent<PlayerUI>().HealthUpdate();
            StartCoroutine(wait((localPlayer.oldhp < localPlayer.health) ? true : false));
            localPlayer.oldhp = localPlayer.health;

        }
        localPlayer.controller.speed = localPlayer.playerSpeed;
        localPlayer.controller.jumpForce = localPlayer.playerJump;

    }
    [System.Serializable]
    public class Player
    {
        public string name;
        public int stdHealth = 100, dopHealth = 0, health = 0;
        public int stdArmor = 0, armor = 0;
        [HideInInspector] public int oldhp;
        public float stdPlayerSpeed = 2000, stdPlayerJump = 10000;
        public float playerSpeed = 0, playerJump = 0;
        public int digRange = 3;
        public int lightRange = 5;
        public int inventorySize = 45;
        public float digForce = 1;
        public List<string> craftingTables = new List<string>();
        public Player2D controller;
        public BuildDig buildDig;
        public ShadowTileMap shadows;
        public List<Item> inventory = new List<Item>();
        public bool canDeath = true;
        public GameObject deadParticles, damageParticles, healParticles;
        public void PlayerStart(Transform transform)
        {
            buildDig = transform.GetComponent<BuildDig>();
            buildDig.mineRange = digRange;
            shadows = FindObjectOfType<ShadowTileMap>();
            if (shadows != null)
            {
                shadows.playerLight = lightRange;
            }
            controller = transform.GetComponent<Player2D>();
            controller.jumpForce = stdPlayerJump + playerJump;
            controller.speed = stdPlayerSpeed + playerSpeed;
            health = stdHealth + health;
            oldhp = health;
        }
    }

    IEnumerator wait(bool plus)
    {
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        List<Color32> renderersCol = new List<Color32>();
        if (GetComponent<SpriteRenderer>() != null)
        {
            renderers.Add(GetComponent<SpriteRenderer>());
            renderersCol.Add(GetComponent<SpriteRenderer>().color);
        }
        var renderersChildren = transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderersChildren.Length; i++)
        {
            renderers.Add(renderersChildren[i]);
            renderersCol.Add(renderersChildren[i].color);
        }
        if (plus == false)
        {
            localPlayer.damageParticles.SetActive(true);
        }
        else
        {
            localPlayer.healParticles.SetActive(true);
        }
        for (int i = 0; i < 2; i++)
        {
            for (int x = 0; x < renderers.Count; x++)
            {
                renderers[x].color = new Color(renderersCol[x].r + 0.5f, renderersCol[x].g, renderersCol[x].b, 0.5f);
            }
            yield return new WaitForSeconds(0.2f);
            for (int x = 0; x < renderers.Count; x++)
            {
                renderers[x].color = new Color(renderersCol[x].r, renderersCol[x].g, renderersCol[x].b, renderersCol[x].a);
            }
            yield return new WaitForSeconds(0.2f);
        }
        localPlayer.damageParticles.SetActive(false);
        localPlayer.healParticles.SetActive(false);
        for (int x = 0; x < renderers.Count; x++)
        {
            renderers[x].color = new Color(renderersCol[x].r, renderersCol[x].g, renderersCol[x].b, renderersCol[x].a);
        }
    }
}




[System.Serializable]
public class Item
{
    public enum itemType {None, Block, Helmet, Body, Hands, Melee, Distance, Dig, Axe, Table, Hammer, Use, Backpack, Foots, Hook};
    public string name = "None", descriptionen, descriptionru;
    public string secondName = "_None_";
    public itemType type;
    public Sprite icon;
    public int value = 1;
    public int maxValue = 100;
    public int inInventory = -1;
    [Space]
    [Header("Is Block")]
    public TileBase tile;
    public Sprite tilesprite;
    public Vector2Int size;
    [Space]
    [Header("In Hands")]
    public Vector2 floatSize;
    public float digLevel;
    [Space]
    [Header("Prefab")]
    public bool onGround;
    public GameObject prefab;
    [Space]
    [Header("Meele")]
    public float distance, coolDown;
    public int damage;
    public float meeleBackForce;
    [Header("Armor")]
    public int armor;
    public string data;
    public Item(string name, string descren, string descrru, string second, itemType itemType, Sprite icon, int value = 1, int maxValue = 999, TileBase til = null, GameObject prefab = null)
    {
        this.name = name;
        this.descriptionen = descren;
        this.descriptionru = descrru;
        this.secondName = second;
        this.type = itemType;
        this.icon = icon;
        this.value = value;
        this.maxValue = maxValue;
        this.tile = til;
        this.prefab = prefab;
    }
    public Item()
    {

    }
    public static Item Clone(Item item)
    {
        return new Item(item.name, item.descriptionen, item.descriptionru, item.secondName, item.type, item.icon, item.value,
            item.maxValue, item.tile, item.prefab)
        { size = item.size, floatSize = item.floatSize,
            digLevel = item.digLevel, damage = item.damage,
            distance = item.distance,
            coolDown = item.coolDown,
            tilesprite = item.tilesprite,
            onGround = item.onGround,
            meeleBackForce = item.meeleBackForce,
            armor = item.armor, data = item.data
        };
    }
}


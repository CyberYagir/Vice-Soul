using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    public List<Block> blocks = new List<Block>();

    public List<Item> items = new List<Item>();

    public List<GameObject> NPCs = new List<GameObject>();

    public List<TileBase> allTiles;

    public List<GameObject> allEntities;

    public List<Craft> crafts;

    public GameObject drop;

    public GameObject shadows;

    private void Start()
    {

        //using (TextWriter writer = new StreamWriter(@"C:\items.txt", false))
        //{
        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        writer.WriteLine(items[i].name);
        //    }
        //    writer.Close();
        //}

        if (Application.isEditor)
        {
            shadows.SetActive(false);
        }
        else
        {
            shadows.SetActive(true);
        }
    }

    public bool firstBoss,secondBoss, thirdBoss, fourBoss;

    public Block GetBlock(TileBase b)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (b == blocks[i].back || b == blocks[i].main)
            {
                return blocks[i];
                break;
            }
        }
        return null;
    } 

    public Item GetItemBySecondName(string secondName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].secondName == secondName)
            {
                return Item.Clone(items[i]);
            }
        }
        return null;
    }
    public Item GetItemByData(string dataContains)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].data.Contains(dataContains))
            {
                return Item.Clone(items[i]);
            }
        }
        return null;
    }
    public Item GetItemBySecondName(string secondName, bool lower)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].secondName.ToLower() == secondName.ToLower())
            {
                return Item.Clone(items[i]);
            }
        }
        return null;
    }
    public int GetItemIDBySecondName(string secondName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].secondName == secondName)
            {
                return i;
            }
        }
        return -1;
    }


    public int GetTileID(TileBase tileBase)
    {
        for (int i = 0; i < allTiles.Count; i++)
        {
            if (allTiles[i] == tileBase)
            return i;
        }
        return -1;
    }

    [System.Serializable]
    public class Block {
        public string name;
        public TileBase main, back;
        public List<string> randomDrop, drop;
        public float force;
        public float digTime;
    }
    [System.Serializable]
    public class Craft
    {
        public string finalItem;
        public int finalItemValue = 1;
        public string craftType;
        public List<CraftItem> crafts = new List<CraftItem>();

        [System.Serializable]
        public class CraftItem {
            public string item;
            public int value;
        }
    }
}

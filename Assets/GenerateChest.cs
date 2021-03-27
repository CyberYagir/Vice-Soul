using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateChest : MonoBehaviour
{
    public int itemcount,maxItemCount;
    public List<RandomItem> items;
    public Chest chest;

    private void Start()
    {
        GetComponent<Entity>().entityID = "_Chest_";
        GetComponent<Entity>().entityName = "Chest";
        maxItemCount = Random.Range(0, 10);
        int p = 0;
        while (itemcount < maxItemCount)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ex == false)
                {
                    if (Random.Range(0, items[i].rareMax) == 0)
                    {
                        items[i].ex = true;
                        Item it = FindObjectOfType<WorldManager>().GetItemBySecondName(items[i].item, true);
                        
                        it.value = Random.Range(items[i].minValue, items[i].maxValue);
                        chest.AddItemChest(FindObjectOfType<PlayerUI>().chest5x5UI.GetComponent<ChestUI>(), it);
                        itemcount++;
                    }
                }
            }
            p++;
            if (p > 30){
                break;
            }
        }
    }


    [System.Serializable]
    public class RandomItem {
        public string item;
        public int minValue, maxValue;
        public int rareMax;
        public bool ex;
    }

}

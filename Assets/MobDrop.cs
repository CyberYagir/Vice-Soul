using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MobDrop : MonoBehaviour
{
    public List<Block> blocks = new List<Block>();
    public MobStats mobStats;
    public bool end;
    private void Start()
    {
        mobStats = GetComponent<MobStats>();
    }

    private void Update()
    {
        if (!end)
        {
            if (mobStats.hp <= 0)
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    int val = Random.Range(blocks[i].min, blocks[i].max + 1);
                    if (val != 0)
                    {
                        GameObject obj = Instantiate(FindObjectOfType<WorldManager>().drop, transform.position, Quaternion.identity);
                        obj.GetComponent<Drop>().item = FindObjectOfType<WorldManager>().GetItemBySecondName(blocks[i].name);
                        obj.GetComponent<Drop>().item.value = val;
                        obj.GetComponent<Drop>().Init();
                    }
                }
                end = true;
            }
        }
    }
    [System.Serializable]
    public class Block {
        public string name;
        public int max, min;
    }

}

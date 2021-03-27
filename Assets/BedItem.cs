using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BedItem : MonoBehaviour
{
    public Image image, back;
    public TextTranslator name, info;
    public GameObject npc;


    private void Start()
    {
        back.color = Color.white;
        var bed = GetComponentInParent<Bed>().GetComponent<Entity>();
        var npcs = FindObjectsOfType<NPC>().ToList().Find(x => x.NPCName == bed.data);

        if (npcs == null)
        {
            bed.data = "";
        }
        else
        {
            if (bed.data == npc.GetComponent<NPC>().NPCName)
            {
                back.color = new Color(0.3975866f, 0.5660378f, 0.3924884f);
            }
        }
    }

    public void Click()
    {
        var bed = GetComponentInParent<Bed>().GetComponent<Entity>();
        if (bed.data == "")
        {
            var beds = FindObjectsOfType<Bed>().ToList().FindAll(x => x.GetComponent<Entity>().data == npc.GetComponent<NPC>().NPCName);
            for (int i = 0; i < beds.Count; i++)
            {
                beds[i].GetComponent<Entity>().data = "";
            }
            npc.transform.position = bed.transform.position + new Vector3(1, 2, 0);
            bed.data = npc.GetComponent<NPC>().NPCName;
            back.color = new Color(0.3975866f, 0.5660378f, 0.3924884f);
            npc.GetComponent<NPC>().start = true;



        }
        else
        {
            var npcs = FindObjectsOfType<NPC>().ToList().Find(x => x.NPCName == bed.data);
            if (npcs != null)
            {
                npcs.start = false;
                npcs.transform.position = new Vector2(bed.transform.position.x + Random.Range(-5, 5), -100);
            }
            back.color = Color.white   ;
            bed.data = "";
            bed.GetComponent<Bed>().UpdateMenu();

        }
    }
}

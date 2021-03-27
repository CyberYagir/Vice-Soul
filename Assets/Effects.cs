using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effects : MonoBehaviour
{
    public List<Effect> AllEffects;
    public List<Effect> effects;
    public Transform effectsContent;
    public Transform effectItem;
    public List<TMPro.TMP_Text> texts;
    private void Start()
    {
        StartCoroutine(waitUpdate());
        //AddEffect(0);
    }

    private void Update()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].time > 0)
            {
                effects[i].time -= Time.deltaTime;
                GetComponent<PlayerStats>().localPlayer.playerJump += effects[i].jump;
                GetComponent<PlayerStats>().localPlayer.playerSpeed += effects[i].speed;
                if (effects[i].time >  Math.Round(effects[i].time) - 0.1f  && effects[i].time <= Math.Round(effects[i].time) + 0.1f)
                {
                    print("Add");
                    GetComponent<PlayerStats>().localPlayer.health += (int)effects[i].regen;
                }
                if (effects[i].activeObject != null)
                {
                    effects[i].activeObject.SetActive(true);
                }
            }
            else
            {

                if (effects[i].activeObject != null)
                {
                    effects[i].activeObject.SetActive(false);
                }
                effects.RemoveAt(i);
                UpdateEffectsUI();
                StopAllCoroutines();
                StartCoroutine(waitUpdate());
            }
        }

        for (int i = 0; i < effects.Count; i++)
        {
            texts[i].text = effects[i].time.ToString("F0");
        }
    }

    public void AddEffect(int id)
    {
        
        for (int p = 0; p < effects.Count; p++)
        {
            if (AllEffects[id].name == effects[p].name)
            {
                effects[p].time = AllEffects[id].time;
                UpdateEffectsUI();
                return;
            }
        }
        effects.Add(new Effect(AllEffects[id]));
        var curr = effects[effects.Count - 1];
        for (int p = 0; p < curr.deleteEffects.Count; p++)
        {
            for (int g = 0; g < 10; g++)
            {
                for (int i = 0; i < effects.Count; i++)
                {
                    if (curr.deleteEffects[p].ToLower() == effects[i].name.ToLower())
                    {
                        if (effects[i].activeObject)
                        {
                            effects[i].activeObject.SetActive(false);
                        }
                        effects.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        UpdateEffectsUI();
    }
    
    void UpdateEffectsUI()
    {
        foreach (Transform item in effectsContent)
        {
            Destroy(item.gameObject);
        }
        texts.Clear();
        for (int i = 0; i < effects.Count; i++)
        {
            GameObject p = Instantiate(effectItem, effectsContent).gameObject;
            p.GetComponent<Image>().sprite = effects[i].icon;
            p.SetActive(true);
            texts.Add(p.GetComponentInChildren<TMPro.TMP_Text>());
            
        }
    }
    IEnumerator waitUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].time > 0)
                {
                    GetComponent<PlayerStats>().TakeDamage(effects[i].damage);
                }
            }
        }
    }
}

[System.Serializable]
public class Effect {
    public string name;
    public Sprite icon;
    public float time;
    public float damage;
    public GameObject activeObject;
    public float jump, speed, regen;
    public List<string> deleteEffects;

    public Effect(Effect effect)
    {
        this.name = effect.name;
        this.icon = effect.icon;
        this.time = effect.time;
        this.damage = effect.damage;
        this.activeObject = effect.activeObject;
        this.jump = effect.jump;
        this.speed = effect.speed;
        this.deleteEffects = effect.deleteEffects;
        this.regen = effect.regen;

    }
}


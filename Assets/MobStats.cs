using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobStats : MonoBehaviour
{
    public string name;
    public int attack, minAttack,maxAttack;
    public float speed, jumpForce;
    public float hp;
    public float minHp, maxHp;
    float oldhp ;
    public float attackCooldown;
    public GameObject damageParticles;

    public void Start()
    {
        hp = (int)Random.Range(minHp, maxHp);
        attack = Random.Range(minAttack, maxAttack);
        oldhp = hp;
    }


    private void FixedUpdate()
    {
        if (hp != oldhp)
        {
            StartCoroutine(wait());
            oldhp = hp;
        }
    }

    IEnumerator wait()
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
        damageParticles.SetActive(true);
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
        damageParticles.SetActive(false);
        for (int x = 0; x < renderers.Count; x++)
        {
            renderers[x].color = new Color(renderersCol[x].r, renderersCol[x].g, renderersCol[x].b, renderersCol[x].a);
        }
    }
}

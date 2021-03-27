using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public Vector3Int pos;
    public float hp, starthp;
    public Animator animator;
    public float speed;
    public string dropId;
    public string seedId = "";
    public string[] dopDrop;
    bool dead = false;
    bool deadstart = false;
    public SpriteRenderer renderer;
    private void Start()
    {
        hp = Random.Range(2, 5);
        starthp = hp;
    }
    private void Update()
    {
        if (dead == false) {
            if (speed > 0)
            {
                animator.Play("Tree");
                speed -= (starthp / hp) * Time.deltaTime;
            }
            animator.speed = speed;
            if (hp <= 0)
            {
                dead = true;
            }
        }
        else
        {
            if (deadstart == false) {
                animator.speed = 1;
                animator.Play("TreeDead");
                StartCoroutine(Dead());
                deadstart = true;
            }
        }
    }


    IEnumerator Dead()
    {
        yield return new WaitForSeconds(1);
        GameObject drop = Instantiate(FindObjectOfType<WorldManager>().drop, transform.position, Quaternion.identity);
        Item it = FindObjectOfType<WorldManager>().GetItemBySecondName(dropId);
        it.value = Random.Range(5, 20);
        drop.GetComponent<Drop>().item = it;
        drop.GetComponent<Drop>().Init();

        if (FindObjectOfType<WorldManager>().GetItemBySecondName(seedId, true) != null)
        {

            GameObject drop1 = Instantiate(FindObjectOfType<WorldManager>().drop, transform.position, Quaternion.identity);
            Item it1 = FindObjectOfType<WorldManager>().GetItemBySecondName(seedId, true);
            it1.value = Random.Range(1, 5);
            drop1.GetComponent<Drop>().item = it1;
            drop1.GetComponent<Drop>().Init();
        }
        for (int i = 0; i < dopDrop.Length; i++)
        {
            GameObject drop1 = Instantiate(FindObjectOfType<WorldManager>().drop, transform.position, Quaternion.identity);
            Item it1 = FindObjectOfType<WorldManager>().GetItemBySecondName(dopDrop[i], true);
            it1.value = Random.Range(1, 5);
            drop1.GetComponent<Drop>().item = it1;
            drop1.GetComponent<Drop>().Init();
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public Collider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(refs());
    }
    IEnumerator refs()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.01f, 0.02f));
            var n = FindObjectsOfType<MetaballParticleClass>();
            for (int i = 0; i < n.Length; i++)
            {
                Physics2D.IgnoreCollision(n[i].GetComponent<CircleCollider2D>(), collider);
            }
            var g = FindObjectsOfType<NPC>();
            for (int i = 0; i < g.Length; i++)
            {
                if (g[i].GetComponent<CircleCollider2D>() != null)
                {
                    Physics2D.IgnoreCollision(g[i].GetComponent<CircleCollider2D>(), collider);
                }
            }
        }
    }
}

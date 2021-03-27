using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public Player2D player;
    public float dist;
    public Light light;
    public GameObject particles;
    public List<Torch> neighbors;
    private void Start()
    {
        particles.SetActive(false);
        light.enabled = false;
        player = FindObjectOfType<Player2D>();
        if (particles.GetComponent<ParticleSystem>() != null)
        {
            particles.GetComponent<ParticleSystem>().Play();
        }
        neighbors = FindObjectsOfType<Torch>().ToList().FindAll(x => Vector2.Distance(transform.position, x.transform.position) < 5 && x.transform != transform);
        light.intensity = 1.1f / (neighbors.Count + 1);
        for (int i = 0; i < neighbors.Count; i++)
        {
            for (int j = 0; j < neighbors[i].neighbors.Count; j++)
            {
                if (neighbors[i].neighbors[j] == null) { neighbors[i].neighbors.RemoveAt(j); };
            }
            neighbors[i].light.intensity = 1.1f / (neighbors[i].neighbors.Count + 1);
        }
    }

    private void FixedUpdate()
    {
        if (player == null) {
            player = FindObjectOfType<Player2D>();
        }
        if (Vector3.Distance(player.transform.position, transform.position) <= dist)
        {
            light.enabled = true;
            particles.SetActive(true);
        }
        else
        {
            particles.SetActive(false);
            light.enabled = false;
        }
    }
}

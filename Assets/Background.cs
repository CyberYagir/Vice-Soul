using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Background : MonoBehaviour
{
    public Player2D player;
    public float y;
    public float speed, yOffcet;
    public Renderer renderer;
    public bool xAx, yAx, ybosLerp, isBack;
    public Vector2 size;
    private void Start()
    {
    }


    private void Update()
    {
        renderer.material.mainTextureScale = size;
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        if (ybosLerp)
        {
            if (transform.position.y < y)
            {
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y + yOffcet, transform.position.z);
            }
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, y, transform.position.z);
        }
        if (yAx)
        {
            print(transform.name);
            renderer.material.mainTextureOffset += (Vector2)(Vector3.up * player.rb.velocity.y * Time.deltaTime * speed);
        }
        if (xAx)
        {
            renderer.material.mainTextureOffset += (Vector2)(Vector3.right * (player.rb.velocity.x) * Time.deltaTime * speed);
        }
    }
}

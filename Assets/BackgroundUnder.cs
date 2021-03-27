using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUnder : MonoBehaviour
{
    public float playerY;
    public Player2D player2D;
    public float speed;
    public Renderer renderer;

    private void Update()
    {
        renderer.material.mainTextureOffset += (Vector2)(Vector3.up * player2D.GetComponent<Rigidbody2D>().velocity.y * Time.deltaTime / speed);

        renderer.material.mainTextureOffset += (Vector2)(Vector3.right * (player2D.GetComponent<Rigidbody2D>().velocity.x) * Time.deltaTime * 0.03f);

        transform.position = new Vector3(player2D.transform.position.x, transform.position.y,transform.position.z);
        if (player2D.transform.position.y <= playerY)
        {
            transform.position = new Vector3(player2D.transform.position.x, player2D.transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(player2D.transform.position.x, playerY,transform.position.z);
        }
    }
}

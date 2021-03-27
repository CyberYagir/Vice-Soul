using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManHole : MonoBehaviour
{
    PlayerUI ui;
    public bool isopen;
    public BoxCollider2D collider2D, defaultColl;
    public SpriteRenderer renderer;
    public Sprite openSprite, closeSprite;
    public GameObject luke;
    private void Start()
    {
        ui = FindObjectOfType<PlayerUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (new Vector2(ui.buildDig.blockPrewiew.transform.position.x, ui.buildDig.blockPrewiew.transform.position.y) == new Vector2(transform.position.x, transform.position.y))
            {
                if (ui.transform.position.y < transform.position.y) {
                    if (Vector2.Distance(ui.transform.position, transform.position) < 1)
                        return;
                }
                collider2D.enabled = !collider2D.enabled;
                defaultColl.enabled = collider2D.enabled;
                isopen = !collider2D.enabled;
                if (isopen)
                {
                    luke.SetActive(true);
                    renderer.sprite = openSprite;
                }
                else
                {
                    luke.SetActive(false);
                    renderer.sprite = closeSprite;
                }
            }
        }
    }

}

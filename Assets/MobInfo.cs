using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MobInfo : MonoBehaviour
{
    public bool over;
    PlayerUI player;
    public MobStats mobStats;
    WorldConfig worldConfig;
    float pivotX = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (mobStats == null)
        {
            mobStats = GetComponent<MobStats>();
        }
        player = FindObjectOfType<PlayerUI>();
        worldConfig = FindObjectOfType<WorldConfig>();
        pivotX = player.infoText.rectTransform.pivot.x;
    }

    private void Update()
    {
        if (over)
        {
            player.infoText.rectTransform.position = Input.mousePosition;
            if (player.infoText.rectTransform.position.x > Screen.width - (player.infoText.rectTransform.sizeDelta.x + 2))
            {
                player.infoText.rectTransform.position = new Vector3(Screen.width - (player.infoText.rectTransform.sizeDelta.x + 2), player.infoText.rectTransform.position.y);
            }
            if (player.infoText.rectTransform.position.x < player.infoText.rectTransform.sizeDelta.x + 2)
            {
                player.infoText.rectTransform.position = new Vector3(player.infoText.rectTransform.sizeDelta.x + 2, player.infoText.rectTransform.position.y);
            }


            if (player.infoText.rectTransform.position.y < (player.infoText.rectTransform.sizeDelta.y + 2))
            {
                player.infoText.rectTransform.position = new Vector3(player.infoText.rectTransform.position.x, (player.infoText.rectTransform.sizeDelta.y + 2));
            }
            if (player.infoText.rectTransform.position.y > Screen.height - (player.infoText.rectTransform.sizeDelta.y + 2))
            {
                player.infoText.rectTransform.position = new Vector3(player.infoText.rectTransform.position.x, Screen.height - (player.infoText.rectTransform.sizeDelta.y + 0.2f));
            }




            if (Input.mousePosition.x > Screen.width / 2)
            {
                player.infoText.rectTransform.pivot = new Vector2(pivotX, 0);
            }
            else
            {
                player.infoText.rectTransform.pivot = new Vector2(1, 0);
            }
            player.infoText.text = mobStats.name;
            if (worldConfig.config.eng)
            {
                player.infoText.text += "\nHealth: " + (int)mobStats.hp + "/" + (int)mobStats.maxHp;
                player.infoText.text += "\nAttack: " + (int)mobStats.minAttack + "/" + (int)mobStats.minAttack;
            }
            else
            {
                player.infoText.text += "\nЗдоровье: " + (int)mobStats.hp + "/" + (int)mobStats.maxHp;
                player.infoText.text += "\nМощность: " + (int)mobStats.minAttack + "/" + (int)mobStats.minAttack;
            }
        }
    }
}

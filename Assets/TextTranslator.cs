using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTranslator : MonoBehaviour
{
    public TMP_Text TMPText;
    public Text Text;
    [TextArea]
    public string engText, rusText;
    public Menu menu;
    public WorldConfig worldConfig;

    private void Start()
    {
        menu = FindObjectOfType<Menu>();
        worldConfig = FindObjectOfType<WorldConfig>();
        if (TMPText != null)
        {
            if (menu != null)
            {
                if (menu.config.eng == true)
                    TMPText.text = engText;
                else
                    TMPText.text = rusText;
            }
            if (worldConfig != null)
            {
                if (worldConfig.config.eng == true)
                    TMPText.text = engText;
                else
                    TMPText.text = rusText;
            }
        }
        if (Text != null)
        {
            if (menu != null)
            {
                if (menu.config.eng == true)
                    Text.text = engText;
                else
                    Text.text = rusText;
            }
            if (worldConfig != null)
            {
                if (worldConfig.config.eng == true)
                    Text.text = engText;
                else
                    Text.text = rusText;
            }
        }
    }
    private void Update()
    {
        if (TMPText != null)
        {
            if (menu != null)
            {
                if (menu.config.eng == true)
                    TMPText.text = engText;
                else
                    TMPText.text = rusText;
            }
            if (worldConfig != null)
            {
                if (worldConfig.config.eng == true)
                    TMPText.text = engText;
                else
                    TMPText.text = rusText;
            }
        }
        if (Text != null)
        {
            if (menu != null)
            {
                if (menu.config.eng == true)
                    Text.text = engText;
                else
                    Text.text = rusText;
            }
            if (worldConfig != null)
            {
                if (worldConfig.config.eng == true)
                    Text.text = engText;
                else
                    Text.text = rusText;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CraftsListItem : MonoBehaviour
{
    public string ID;
    public TMP_Text craftItemName;
    public Image image;

    public GameObject subCraftsContent, subCraft;

    public WorldManager worldManager;

    private void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
    }

}

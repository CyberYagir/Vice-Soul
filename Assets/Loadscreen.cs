using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loadscreen : MonoBehaviour
{
    public WorldGenerator worldGenerator;


    public GameObject holder;
    public GameObject item;
    public Image itemImage;
    [Space]
    public bool seeCounter;
    public TextTranslator text;
    private void Start()
    {
        GridLayoutGroup g = holder.GetComponent<GridLayoutGroup>();
        g.cellSize = new Vector2(650 / worldGenerator.chunksHorizontalCount, 650 / worldGenerator.chunksVerticalCount);
        
    }
    private void Update()
    {
        foreach (Transform item in holder.transform)
        {
            Destroy(item.gameObject);
        }
        int gened = 0;
        int chunktoload = (worldGenerator.chunksHorizontalCount - (((int)(worldGenerator.chunksHorizontalCount * 0.3f)) + (int)(worldGenerator.chunksHorizontalCount * 0.3f))) * 2;
        for (int x = 0; x < worldGenerator.chunksHorizontalCount; x++)
        {
            for (int y = worldGenerator.chunksVerticalCount - 1; y >= 0; y--)
            {
                if (worldGenerator.chunks[x][y].generated == false && worldGenerator.chunks[x][y].chunkTexture == null)
                {
                    itemImage.color = new Color(0, 0, 0, 0.5f);
                }
                else
                 if (worldGenerator.chunks[x][y].generated == false && worldGenerator.chunks[x][y].chunkTexture != null)
                {

                    itemImage.color = new Color(0.2f, 1f, 0.2f, 0.5f);
                }
                else
                if (worldGenerator.chunks[x][y].generated == true && worldGenerator.chunks[x][y].chunkTexture != null)
                {
                    itemImage.color = new Color(0, 1, 1, 0.5f);
                    gened++;
                }
                GameObject p = Instantiate(item);
                p.transform.position = holder.transform.position + new Vector3(x * (650 / worldGenerator.chunksHorizontalCount), (worldGenerator.chunksVerticalCount * (650 / worldGenerator.chunksVerticalCount)) - y * (650 / worldGenerator.chunksVerticalCount));
                if (p.transform.GetComponent<RectTransform>() != null)
                    p.transform.GetComponent<RectTransform>().sizeDelta = new Vector3((650 / worldGenerator.chunksHorizontalCount), (650 / worldGenerator.chunksVerticalCount));
                p.SetActive(true);
                try
                {

                    p.transform.SetParent(holder.transform);

                }
                catch (System.Exception)
                {
                    continue;
                }

            }
        }
        if (seeCounter)
        {
            text.rusText = "Фрагментация " + gened + "/" + chunktoload + " (Первая генерация может быть долгой.)";
            text.engText = "Loading " + gened + "/" + chunktoload + " (The first generation can be long.)";
        }
    }

}

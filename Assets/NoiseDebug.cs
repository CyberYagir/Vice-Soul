using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseDebug : MonoBehaviour
{
    public RawImage image;


    public WorldGenerator worldGenerator;
    private void Start()
    {
        worldGenerator.offcetX = worldGenerator.offcetX + worldGenerator.dopOffcetX;
        worldGenerator.offcetY = worldGenerator.offcetY + worldGenerator.dopOffcetY;
        worldGenerator.noiseTex = new Texture2D(worldGenerator.pixWidth, worldGenerator.pixHeight);
        worldGenerator.pix = new Color[worldGenerator.noiseTex.width * worldGenerator.noiseTex.height];

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            worldGenerator.noiseTex = new Texture2D(worldGenerator.pixWidth, worldGenerator.pixHeight);
            worldGenerator.pix = new Color[worldGenerator.noiseTex.width * worldGenerator.noiseTex.height];
            print("update");
            var p = worldGenerator._CalcNoise(new Vector2Int(0, 0));
            p.filterMode = FilterMode.Point;
            image.texture = p;
            
        }
    }

    public void Press()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidTime : MonoBehaviour
{
    public WorldLive manager;

    public Water2D.Water2D_Spawner water2;

    public Color startFill, startStroke;

    private void Start()
    {
        manager = FindObjectOfType<WorldLive>();
        startFill = water2.FillColor;
        startStroke = water2.StrokeColor;
    }

    private void FixedUpdate()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<WorldLive>();
        }
        water2.FillColor = new Color(startFill.r * (manager.intensivity +1), startFill.g * (manager.intensivity + 1), startFill.b * (manager.intensivity + 1), 0.8f);
        water2.StrokeColor = new Color(startStroke.r * (manager.intensivity + 1), startStroke.g * (manager.intensivity + 1), startStroke.b * (manager.intensivity + 1), 0.8f);
    }
}

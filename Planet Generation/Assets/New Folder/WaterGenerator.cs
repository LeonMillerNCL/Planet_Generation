using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGenerator 
{
    WaterSettings settings;

    public WaterGenerator(WaterSettings settings)
    {
        this.settings = settings;
    }

    public Vector3 CalculatePointonWater(Vector3 pointOnUnitSphere)
    {
        return pointOnUnitSphere * settings.waterRadius;
    }
}

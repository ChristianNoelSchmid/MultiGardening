using System;
using UnityEngine;

// A struct representing all information
// related to a PlantGrowth's current stage
[Serializable]
public struct PlantStage
{
    public Sprite sprite;
    public float timeIntervalSec;
    public float minSize;
}
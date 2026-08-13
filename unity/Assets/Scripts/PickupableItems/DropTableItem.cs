using System;
using UnityEngine;

[Serializable]
public class DropTableItem {
    public GameObject itemPf;
    public int minCount = 1;
    public int maxCount = 1;
    [Range(0f, 1f)] public float probability = 1f;
}

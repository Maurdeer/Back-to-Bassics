using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorLineSpawner : Conductable
{
    [SerializeField] private GameObject line;

    protected override void OnFullBeat()
    {
        Instantiate(line, transform);
    }
}

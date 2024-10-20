using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : Conductable
{
    [SerializeField] private IndicatorLineSpawner leftSpawner;
    [SerializeField] private IndicatorLineSpawner rightSpawner;

    private void Start()
    {
        Enable();

        leftSpawner.Enable();
        rightSpawner.Enable();
    }
}
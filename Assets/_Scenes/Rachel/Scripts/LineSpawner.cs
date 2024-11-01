using UnityEngine;

public class IndicatorLineSpawner : Conductable
{
    [SerializeField] private GameObject line;

    private void Start()
    {
        Enable();
    }

    protected override void OnFullBeat()
    {
        Instantiate(line, transform);
    }
}

using UnityEngine;

public class IndicatorLineSpawner : Conductable
{
    [SerializeField] private bool isRight;
    [SerializeField] private GameObject line;

    private void Start()
    {
        Enable();
    }

    protected override void OnFullBeat()
    {
        IndicatorLine indicatorLine = Instantiate(line, transform).GetComponent<IndicatorLine>();
        indicatorLine.isRight = isRight;
    }
}

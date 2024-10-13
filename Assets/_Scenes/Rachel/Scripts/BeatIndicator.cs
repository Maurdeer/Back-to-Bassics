using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : Conductable
{
    [Header("Indicator")]
    [SerializeField] private Slider rightSlider;
    [SerializeField] private Slider leftSlider;

    private void Start()
    {
        Enable();
    }

    private IEnumerator IBeat()
    {
        float elapsedTime = 0f;

        float rightStartValue = 1f;
        float leftStartValue = 1f;

        float targetValue = 0.5f;

        while (elapsedTime < Conductor.Instance.spb)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / Conductor.Instance.spb;

            rightSlider.value = Mathf.Lerp(rightStartValue, targetValue, t);
            leftSlider.value = Mathf.Lerp(leftStartValue, targetValue, t);

            yield return null;
        }

        rightSlider.value = 1f;
        leftSlider.value = 1f;
    }

    protected override void OnFullBeat()
    {
        StartCoroutine(IBeat());
    }
}

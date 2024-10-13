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
        float halfSpb = Conductor.Instance.spb / 2f;
        float elapsedTime = 0f;

        float rightStartValue = 0.5f;
        float leftStartValue = 0.5f;

        float targetValue = 1f;

        while (elapsedTime < halfSpb)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfSpb;

            rightSlider.value = Mathf.Lerp(rightStartValue, targetValue, t);
            leftSlider.value = Mathf.Lerp(leftStartValue, targetValue, t);

            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < halfSpb)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfSpb;

            rightSlider.value = Mathf.Lerp(targetValue, rightStartValue, t);
            leftSlider.value = Mathf.Lerp(targetValue, leftStartValue, t);

            yield return null;
        }
    }


    protected override void OnFullBeat()
    {
        StartCoroutine(IBeat());
    }
}

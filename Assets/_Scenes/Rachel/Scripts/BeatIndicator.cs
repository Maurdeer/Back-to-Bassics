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

        float rightStartValue = 0.89f;
        float leftStartValue = 0.89f;

        float targetValue = 1f;

        while (elapsedTime < halfSpb)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfSpb;

            float curvedT = Mathf.SmoothStep(0.89f, 1f, t);

            rightSlider.value = Mathf.Lerp(rightStartValue, targetValue, curvedT);
            leftSlider.value = Mathf.Lerp(leftStartValue, targetValue, curvedT);

            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < halfSpb)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfSpb;

            float curvedT = Mathf.SmoothStep(0f, 1f, t);

            rightSlider.value = Mathf.Lerp(targetValue, rightStartValue, curvedT);
            leftSlider.value = Mathf.Lerp(targetValue, rightStartValue, curvedT);

            yield return null;
        }
    }



    protected override void OnFullBeat()
    {
        StartCoroutine(IBeat());
    }
}

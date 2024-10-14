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

        Vector3 originalScale = transform.localScale;

        // pulse out
        while (elapsedTime < halfSpb)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfSpb;
            float curvedT = Mathf.SmoothStep(0.9f, 1f, t);

            rightSlider.value = Mathf.Lerp(rightStartValue, targetValue, curvedT);
            leftSlider.value = Mathf.Lerp(leftStartValue, targetValue, curvedT);

            float scalePulse = Mathf.Lerp(1f, 1.05f, curvedT);
            transform.localScale = originalScale * scalePulse;

            yield return null;
        }

        elapsedTime = 0f;

        // fall back
        while (elapsedTime < halfSpb)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfSpb;
            float curvedT = Mathf.SmoothStep(0f, 1f, t);

            rightSlider.value = Mathf.Lerp(targetValue, rightStartValue, curvedT);
            leftSlider.value = Mathf.Lerp(targetValue, rightStartValue, curvedT);

            float scalePulse = Mathf.Lerp(1.05f, 1f, curvedT);
            transform.localScale = originalScale * scalePulse;

            yield return null;
        }

        transform.localScale = originalScale;
    }




    protected override void OnFullBeat()
    {
        StartCoroutine(IBeat());
    }
}

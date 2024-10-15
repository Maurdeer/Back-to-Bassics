using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : Conductable
{
    [Header("Indicator")]
    [SerializeField][Range(0.7f, 1.0f)] private float startValue = 0.7f;
    [SerializeField] private Slider rightSlider;
    [SerializeField] private Slider leftSlider;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;

        Enable();
    }

    private float halfSpb = 0f;
    private bool isBeat = false;

    protected override void OnFullBeat()
    {
        halfSpb = Conductor.Instance.spb / 2f;
        isBeat = true;
    }

    private float elapsedTime = 0f;
    private readonly float targetValue = 1f;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (isBeat)
        {
            if (elapsedTime < halfSpb)
            {
                float t = elapsedTime / halfSpb;
                float curvedT = Mathf.SmoothStep(0.9f, 1f, t);

                rightSlider.value = Mathf.Lerp(startValue, targetValue, curvedT);
                leftSlider.value = Mathf.Lerp(startValue, targetValue, curvedT);

                float scalePulse = Mathf.Lerp(1f, 1.05f, curvedT);
                transform.localScale = originalScale * scalePulse;
            }
            else
            {
                isBeat = false;
                elapsedTime = 0f;
            }
        }
        else
        {
            if (elapsedTime < halfSpb)
            {
                float t = elapsedTime / halfSpb;
                float curvedT = Mathf.SmoothStep(0f, 1f, t);

                rightSlider.value = Mathf.Lerp(targetValue, startValue, curvedT);
                leftSlider.value = Mathf.Lerp(targetValue, startValue, curvedT);

                float scalePulse = Mathf.Lerp(1.05f, 1f, curvedT);
                transform.localScale = originalScale * scalePulse;
            }
            else
            {
                isBeat = true;
                elapsedTime = 0f;
            }
        }
    }
}
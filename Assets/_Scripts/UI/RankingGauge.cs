using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class RankingGauge : MonoBehaviour
{
    [Header("Specs")]
    [SerializeField] private float lowerBound;
    [SerializeField] private float upperBound;

    [Header("UI Elements")]
    [SerializeField] private RectTransform bar;

    public Coroutine UpdateGauge(double from, double to, double max)
    {
        return StartCoroutine(GaugeFilling(from, to, max));
    }

    public void SetGauge(float fill)
    {
        if (fill > 1) fill = 1;
        float length = fill * upperBound + (1 - fill) * lowerBound;
        bar.anchoredPosition = new Vector2(length, bar.anchoredPosition.y);
    }

    private IEnumerator GaugeFilling(double from, double to, double max)
    {
        double fill = 0;
        double length;
        while (from < to)
        {
            from+=100;
            fill = from / max;
            if (fill > 1) fill = 1;
            length = fill * upperBound + (1 - fill) * lowerBound;
            bar.anchoredPosition = new Vector2((float)length, bar.anchoredPosition.y);
            yield return new WaitForEndOfFrame();
        }
        fill = to / max;
        if (fill > 1) fill = 1;
        length = fill * upperBound + (1 - fill) * lowerBound;
        bar.anchoredPosition = new Vector2((float)length, bar.anchoredPosition.y);
    }
}

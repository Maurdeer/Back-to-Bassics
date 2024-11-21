using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboGaugeBar : MonoBehaviour
{
    [Header("Specs")]
    [SerializeField] private float lowerBound;
    [SerializeField] private float upperBound;

    [Header("UI Elements")] 
    [SerializeField] private RectTransform bar;
    [SerializeField] private TextMeshProUGUI gaugeText;

    private void Start()
    {
        UpdateGauge(0, 100);
    }
    public void UpdateGauge(float curr, float max)
    {
        float fill = curr / max;
        float height = fill * upperBound + (1 - fill) * lowerBound;
        bar.anchoredPosition = new Vector2(bar.anchoredPosition.x, height);
        gaugeText.text = $"{curr}/{max}";
    }
}

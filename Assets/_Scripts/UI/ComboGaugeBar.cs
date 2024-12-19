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
    [SerializeField] private GameObject ultimateComboPanel;

    private void Start()
    {
        UpdateGauge(0, 100);
    }
    public void UpdateGauge(float curr, float max)
    {
        float fill = curr / max;
        float height = fill * upperBound + (1 - fill) * lowerBound;
        bar.anchoredPosition = new Vector2(bar.anchoredPosition.x, height);
        if (fill == 1)
        {
            gaugeText.text = $"ULTIMATE\r\nREADY";
            gaugeText.color = new Color32(229, 49, 130, 255);
        }
        else
        {
            gaugeText.text = $"{curr}/{max}";
            gaugeText.color = new Color32(9, 174, 217, 255);
        }
        
    }
}

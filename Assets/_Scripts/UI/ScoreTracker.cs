using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_comboMultiplierText;
    public void UpdateScore(ulong score)
    {
        m_scoreText.text = score.ToString("D12");    
    }
    public void UpdateMultiplier(uint multiplier)
    {
        m_comboMultiplierText.text = $"{multiplier}x";
    }
}

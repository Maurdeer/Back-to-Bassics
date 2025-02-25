using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_comboMultiplierText;
    [SerializeField] private TextMeshProUGUI m_timeMultiplierText;
    private float decayTH;
    private float hardDecay;
    private float softDecay;
    private float currTimeMultiplierValue;
    private float timeStarted;
    private bool runTimeMultiplier;
    private void Update()
    {
        if (!runTimeMultiplier) return;

        // Calcualte Time Multiplier Every Frame (-_-)
        float secondsPassed = Time.time - timeStarted;
        if (secondsPassed <= decayTH)
        {
            currTimeMultiplierValue = 3f - secondsPassed * hardDecay;
        }
        else
        {
            float decay = (secondsPassed - decayTH) * softDecay; // 5 minutes
            currTimeMultiplierValue = Mathf.Clamp(1f - decay, 0.01f, 1f);
        }

        // Update Tracker
        m_timeMultiplierText.text = currTimeMultiplierValue.ToString("0.0") + "x";
    }
    public void UpdateScore(ulong score)
    {
        m_scoreText.text = score.ToString("D10");    
    }
    public void UpdateMultiplier(uint multiplier)
    {
        m_comboMultiplierText.text = $"{multiplier}x";
    }
    public void StartTimeMultiplier(float clockDecayTH)
    {
        runTimeMultiplier = true;
        currTimeMultiplierValue = 3f;
        timeStarted = Time.time;
        decayTH = clockDecayTH;
        hardDecay = 2f / decayTH;
        softDecay = 0.99f / 300f; // 5 minutes after hard decay till 0.01x
    }
    public float StopAndGetTimeMultiplier()
    {
        runTimeMultiplier = false;
        return currTimeMultiplierValue;
    }
}

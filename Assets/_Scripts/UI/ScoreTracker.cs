using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    [Header("Specifications")]
    [SerializeField] private Color NonDecayingMultiplierColor;
    [SerializeField] private Color DecayingMultiplierColor;
    [SerializeField] private Color SevereDecayingMultiplierColor;
    [Header("References")]
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_comboMultiplierText;
    [SerializeField] private TextMeshProUGUI m_timeMultiplierText;
    private Animator m_animator;
    private float waitTH;
    private float decayTH;
    private float hardDecay;
    private float softDecay;
    private float currTimeMultiplierValue;
    private float timeStarted;
    private bool runTimeMultiplier;
    private Coroutine textUpdater;
    private ulong currScore;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();  
    }
    private void Update()
    {
        if (!runTimeMultiplier) return;

        // Calcualte Time Multiplier Every Frame (-_-)
        float secondsPassed = Time.time - timeStarted;
        if (secondsPassed < waitTH)
        {
            currTimeMultiplierValue = 3f;
            m_timeMultiplierText.color = NonDecayingMultiplierColor;
        }
        else if (secondsPassed <= decayTH)
        {
            float decay = (secondsPassed - waitTH) * hardDecay;
            currTimeMultiplierValue = 3f - decay;
            m_timeMultiplierText.color = DecayingMultiplierColor;
        }
        else
        {
            float decay = (secondsPassed - decayTH) * softDecay; // 5 minutes
            currTimeMultiplierValue = Mathf.Clamp(1f - decay, 0.1f, 1f);
            m_timeMultiplierText.color = SevereDecayingMultiplierColor;
        }

        // Update Tracker
        m_timeMultiplierText.text = currTimeMultiplierValue.ToString("0.0") + "x";
    }
    public void UpdateScore(ulong score)
    {
        if (textUpdater != null)
        {
            StopCoroutine(textUpdater);
            m_scoreText.text = currScore.ToString("D10");
        }

        currScore = score;
        textUpdater = StartCoroutine(TextUpdater(currScore, score));
    }
    public void UpdateMultiplier(uint multiplier)
    {
        m_comboMultiplierText.text = $"{multiplier}x";
        if (multiplier <= 1)
        {
            m_animator.Play("shake");
        }
    }
    public void StartTimeMultiplier(float clockDelayTH, float clockDecayTH)
    {
        runTimeMultiplier = true;
        currTimeMultiplierValue = 3f;
        timeStarted = Time.time;
        waitTH = clockDelayTH;
        decayTH = clockDecayTH;
        hardDecay = 2f / (decayTH - waitTH);
        softDecay = 0.9f / 300f; // 5 minutes after hard decay till 0.01x
    }
    public ulong StopAndGetFinalScore()
    {
        runTimeMultiplier = false;
        ulong finalScore = (ulong)(currScore * double.Parse(currTimeMultiplierValue.ToString("0.0")));
        UpdateScore(finalScore);
        return currScore;
    }
    private IEnumerator TextUpdater(ulong from, ulong to)
    {
        if (from < to)
        {
            // increasing
            while (from < to)
            {
                from+=100;
                m_scoreText.color = NonDecayingMultiplierColor;
                m_scoreText.text = from.ToString("D10");
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            // decreasing
            while (from > to && from > 100)
            {
                from-=100;
                m_scoreText.color = SevereDecayingMultiplierColor;
                m_scoreText.text = from.ToString("D10");
                yield return new WaitForEndOfFrame();
            }
        }
        m_scoreText.color = Color.black;
        m_scoreText.text = to.ToString("D10");
    }
}

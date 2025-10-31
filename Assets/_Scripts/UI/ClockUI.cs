using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_clockText;
    private bool m_clockActive;
    private float m_secondsPassed;
    public float SecondsPassed => m_secondsPassed;
    private float m_startTime;
    private void Update()
    {
        m_secondsPassed = Time.time - m_startTime;
        if (m_clockActive) m_clockText.text = $"{(int)m_secondsPassed / 60}:{m_secondsPassed % 60:00}";
    }
    public void StartClock()
    {
        m_clockActive = true;
        m_startTime = Time.time;
        m_secondsPassed = 0;
    }
    public void StopClock()
    {
        m_clockActive = false;
    }
}

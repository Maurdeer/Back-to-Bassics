using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PersistentDataTracker : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TextMeshProUGUI m_profileNameText;
    [SerializeField] private Transform m_enemyScoreTexts;
    private ulong[] m_enemyScores;
    private string[] m_enemyRanks;
    private ulong totalScore;
    private void Awake()
    {
        m_enemyScores = new ulong[4];
        m_enemyRanks = new string[4];
        totalScore = 0;
    }
    public void UpdateEnemyScore(int id, ulong score, string rank)
    {
        m_enemyScores[id] = score;
        m_enemyRanks[id] = rank;
        RefreshTotalScore();
        m_enemyScoreTexts.GetChild(id).GetComponentInChildren<TextMeshProUGUI>().text = score.ToString("D10");
    }
    public void LoadData(GameData data)
    {
        m_profileNameText.text = data.profileName;
        int i = 0;
        foreach (TextMeshProUGUI child in m_enemyScoreTexts.GetComponentsInChildren<TextMeshProUGUI>())
        {
            m_enemyScores[i] = data.enemyScore[i];
            m_enemyRanks[i] = data.enemyRank[i];
            child.text = data.enemyScore[i].ToString("D10");
            i++;
        }
        RefreshTotalScore(); // Inefficent, but modular
    }

    public void SaveData(GameData data)
    {
        for (int i = 0; i < m_enemyScores.Length; i++)
        {
            data.enemyScore[i] = m_enemyScores[i];
            data.enemyRank[i] = m_enemyRanks[i];
        }
        RefreshTotalScore();
        data.totalScore = totalScore;
    }

    private void RefreshTotalScore()
    {
        totalScore = 0;
        foreach (ulong score in m_enemyScores)
        {
            totalScore += score;
        }
    }
}

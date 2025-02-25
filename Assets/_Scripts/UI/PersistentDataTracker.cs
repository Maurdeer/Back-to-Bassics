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
    private void Awake()
    {
        m_enemyScores = new ulong[4];
    }
    public void UpdateEnemyScore(int id, ulong score)
    {
        m_enemyScores[id] = score;
        m_enemyScoreTexts.GetChild(id).GetComponentInChildren<TextMeshProUGUI>().text = score.ToString("D12");
    }
    public void LoadData(GameData data)
    {
        m_profileNameText.text = data.profileName;
        int i = 0;
        foreach (Transform enemyScoreText in m_enemyScoreTexts)
        {
            m_enemyScores[i] = data.enemyScore[i];
            enemyScoreText.GetComponentInChildren<TextMeshProUGUI>().text = data.enemyScore[i].ToString("D12");
            i++;
        }
    }

    public void SaveData(GameData data)
    {
        int i = 0;
        foreach (ulong enemyScore in m_enemyScores)
        {
            data.enemyScore[i++] = enemyScore;
        }
    }
}

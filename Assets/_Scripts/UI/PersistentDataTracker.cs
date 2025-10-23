using TMPro;
using UnityEngine;

public class PersistentDataTracker : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TextMeshProUGUI m_profileNameText;
    [SerializeField] private Transform m_enemyScoreTexts;
    [SerializeField] private Transform m_enemyRankTexts;
    private ulong[] m_enemyScores;
    private string[] m_enemyRanks;
    private ulong totalScore;
    const int number_of_enemies = 5;
    private void Awake()
    {
        m_enemyScores = new ulong[number_of_enemies];
        m_enemyRanks = new string[number_of_enemies] { "-", "-", "-", "-", "-"};
        totalScore = 0;
    }
    public void UpdateEnemyScore(int id, ulong score, string rank)
    {
        if (m_enemyScores[id] > score) return; // Don't update if score is worse!
        m_enemyScores[id] = score;
        m_enemyRanks[id] = rank;
        RefreshTotalScore();
        m_enemyScoreTexts.GetChild(id).GetComponentInChildren<TextMeshProUGUI>().text = score.ToString("D10");
        m_enemyRankTexts.GetChild(id).GetComponentInChildren<TextMeshProUGUI>().text = rank;
    }
    public void LoadData(GameData data)
    {
        m_profileNameText.text = data.profileName;
        TextMeshProUGUI[] scoreTexts = m_enemyScoreTexts.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] rankTexts = m_enemyRankTexts.GetComponentsInChildren<TextMeshProUGUI>();

        if (scoreTexts == null || rankTexts == null)
        {
            if (scoreTexts == null)
            {
                Debug.LogError("Couldn't Find Scores Text Panels");
            }
            if (rankTexts == null)
            {
                Debug.LogError("Couldn't Find Rank Text Panels");
            }
            for (int i = 0; i < number_of_enemies; i++)
            {
                m_enemyScores[i] = data.enemyScore[i];
                m_enemyRanks[i] = data.enemyRank[i];
            }
            RefreshTotalScore();
            return;
        }

        for (int i = 0; i < number_of_enemies; i++)
        {
            m_enemyScores[i] = data.enemyScore[i];
            m_enemyRanks[i] = data.enemyRank[i];
            scoreTexts[i].text = data.enemyScore[i].ToString("D10");
            rankTexts[i].text = data.enemyRank[i];
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

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
    private void Awake()
    {
        m_enemyScores = new ulong[4];
        m_enemyRanks = new string[4] { "", "", "", ""};
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
        TextMeshProUGUI[] scoreTexts = m_enemyScoreTexts.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] rankTexts = m_enemyRankTexts.GetComponentsInChildren<TextMeshProUGUI>();
        if (scoreTexts == null)
        {
            Debug.LogError("Couldn't Find Scores Text Panels");
            return;
        }
        if (rankTexts == null)
        {
            Debug.LogError("Couldn't Find Rank Text Panels");
            return;
        }
        for (int i = 0; i < scoreTexts.Length; i++)
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

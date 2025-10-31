using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersistentDataReader : MonoBehaviour, IDataPersistence
{
    [SerializeField] private int number_of_enemies = 5;
    [SerializeField] private TextMeshProUGUI m_profileNameText;
    [SerializeField] private EnemyBattlePawnData[] enemyBattlePawnDatas;
    [SerializeField] private Transform characterPanels;

    private void Awake()
    {
        foreach (Transform child in characterPanels)
        {
            child.gameObject.SetActive(false);
        }
        characterPanels.GetChild(0).gameObject.SetActive(true);
    }

    public void LoadData(GameData data)
    {
        m_profileNameText.text = data.profileName;
        for (int i = 0; i < characterPanels.childCount; i++)
        {
            Transform child = characterPanels.GetChild(i);
            TextMeshProUGUI characterNameText = child.Find("CharacterName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = child.Find("CharacterDescription").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = child.Find("Score").GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI rankText = child.Find("Rank").GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI requirements = child.Find("Requirements").GetComponent<TextMeshProUGUI>();
            characterNameText.text = enemyBattlePawnDatas[i].name;
            descriptionText.text = enemyBattlePawnDatas[i].Lore;
            scoreText.text = data.enemyScore[i].ToString("D10");
            rankText.text = data.enemyRank[i];
            ulong divide = enemyBattlePawnDatas[i].SRankMax / 5;
            ulong currScore = enemyBattlePawnDatas[i].SRankMax;
            requirements.text = "";
            requirements.text += $"[S] {currScore}\n";
            currScore -= divide;
            requirements.text += $"[A] {currScore}\n";
            currScore -= divide;
            requirements.text += $"[B] {currScore}\n";
            currScore -= divide;
            requirements.text += $"[C] {currScore}\n";
            currScore -= divide;
            requirements.text += $"[D] {currScore}\n";
            requirements.text += $"[Clock Delay] {(int)enemyBattlePawnDatas[i].ClockDelayTH / 60}:{enemyBattlePawnDatas[i].ClockDelayTH % 60:00}\n";
            requirements.text += $"[Clock Decay] {(int)enemyBattlePawnDatas[i].ClockDecayTH / 60}:{enemyBattlePawnDatas[i].ClockDecayTH % 60:00}\n";
        }
    }

    public void SaveData(GameData data)
    {
        // No need to save
    }

    public void Save()
    {
        DataPersistenceManager.Instance.SaveGame();
    }
}

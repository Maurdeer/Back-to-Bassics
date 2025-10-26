using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersistentDataReader : MonoBehaviour, IDataPersistence
{
    [SerializeField] private int number_of_enemies = 5;
    [SerializeField] private TextMeshProUGUI m_profileNameText;
    [SerializeField] private EnemyBattlePawnData[] enemyBattlePawnDatas;
    [SerializeField] private Transform[] characterPanels;
    

    public void LoadData(GameData data)
    {
        m_profileNameText.text = data.profileName;
        for (int i = 0; i < characterPanels.Length; i++)
        {
            TextMeshProUGUI characterNameText = characterPanels[i].Find("CharacterName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = characterPanels[i].Find("CharacterDescription").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = characterPanels[i].Find("Score").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI rankText = characterPanels[i].Find("Rank").GetComponent<TextMeshProUGUI>();

            characterNameText.text = enemyBattlePawnDatas[i].name;
            descriptionText.text = data.enemyScore[i].ToString("D10");
            scoreText.text = enemyBattlePawnDatas[i].Lore;
            rankText.text = data.enemyRank[i];
        }
    }

    public void SaveData(GameData data)
    {
        // No need to save
    }
}

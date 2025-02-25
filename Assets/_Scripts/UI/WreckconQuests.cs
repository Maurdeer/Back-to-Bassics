using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WreckconQuests : Singleton<WreckconQuests>, IDataPersistence
{
    [SerializeField] private Transform m_pointDelgationHolder;
    [SerializeField] private Sprite notAchievedImage;
    [SerializeField] private Sprite achievedImage;
    private void Awake()
    {
        InitializeSingleton();
    }
    public void MarkAchievement(int id)
    {
        m_pointDelgationHolder.GetChild(id).GetComponent<Image>().sprite = achievedImage;
    }
    public void LoadData(GameData data)
    {
        int i = 0;
        foreach (Transform child in m_pointDelgationHolder)
        {
            child.GetComponent<Image>().sprite = data.wreckconQuests[i++] ? achievedImage : notAchievedImage;
        }
    }

    public void SaveData(GameData data)
    {
        int i = 0;
        foreach (GameObject child in m_pointDelgationHolder)
        {
            data.wreckconQuests[i++] = child.GetComponent<Image>().sprite == achievedImage;
        }
    }
}

public enum Quests
{
    BASSICS_BEAT = 0,
    BASSICS_SPECIAL,
    BASSICS_S,
    BASSICS_A,

    SMALLFRY_BEAT,
    SMALLFRY_SPECIAL,
    SMALLFRY_S,
    SMALLFRY_A,

    TURBOTOP_BEAT,
    TURBOTOP_SPECIAL,
    TURBOTOP_S,
    TURBOTOP_A,

    KINGSAL_BEAT,
    KINGSAL_SPECIAL,
    KINGSAL_S,
    KINGSAL_A,
}

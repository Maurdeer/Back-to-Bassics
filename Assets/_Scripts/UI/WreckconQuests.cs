using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WreckconQuests : Singleton<WreckconQuests>, IDataPersistence
{
    [SerializeField] private Transform m_pointDelgationHolder;
    [SerializeField] private Sprite notAchievedImage;
    [SerializeField] private Sprite achievedImage;
    [SerializeField] private TextMeshProUGUI m_ticketText;
    private int tickets;
    public bool[] truthArray;
    private void Awake()
    {
        InitializeSingleton();
    }
    public void MarkAchievement(int id)
    {
        // Assumes that we only have 16 tasks with perfect order of ticket value
        Image image = m_pointDelgationHolder.GetChild(id).GetComponent<Image>();
        if (image.sprite == achievedImage) return;
        image.sprite = achievedImage;
        if (id % 4 == 0)
        {
            // Defeated a Boss
            tickets += 10;
            truthArray[id / 4] = true;
        }
        else
        {
            // Did another task
            tickets += 5;
        }
        
        m_ticketText.text = tickets.ToString();  
    }
    public void LoadData(GameData data)
    {
        int i = 0;
        foreach (Transform child in m_pointDelgationHolder)
        {
            child.GetComponent<Image>().sprite = data.wreckconQuests[i++] ? achievedImage : notAchievedImage;
        }

        tickets = data.tickets;
        m_ticketText.text = tickets.ToString();
        truthArray = data.truthArray;
    }

    public void SaveData(GameData data)
    {
        int i = 0;
        foreach (Transform child in m_pointDelgationHolder)
        {
            data.wreckconQuests[i++] = child.GetComponent<Image>().sprite == achievedImage;
        }

        data.tickets = tickets;
        // (Ryan) Keeping this as a reminder, Passed by referenced, no need to save!
        //data.truthArray = truthArray;
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

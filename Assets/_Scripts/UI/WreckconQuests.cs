using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckconQuests : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Transform m_pointDelgationHolder;
    public void LoadData(GameData data)
    {
        int i = 0;
        foreach (GameObject child in m_pointDelgationHolder)
        {
            child.gameObject.SetActive(data.wreckconQuests[i++]);
        }
    }

    public void SaveData(GameData data)
    {
        int i = 0;
        foreach (GameObject child in m_pointDelgationHolder)
        {
            data.wreckconQuests[i++] = child.gameObject.activeSelf;
        }
    }
}

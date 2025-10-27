using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruthArrayMaster : Singleton<TruthArrayMaster>, IDataPersistence
{
    [SerializeField] private TransformsToDeactivate[] transformsToDeactivate;
    private bool[] truthArray;
    private void Awake()
    {
        InitializeSingleton();
    }
    public void UpdateTruth(int idx)
    {
        if (idx < 0 && idx >= truthArray.Length)
        {
            Debug.LogError("Breh really?");
            return;
        }
        truthArray[idx] = true;
        DataPersistenceManager.Instance.SaveGame();
    }
    public void LoadData(GameData data)
    {
        truthArray = new bool[data.truthArray.Length];
        for (int i = 0; i < data.truthArray.Length; i++)
        {
            truthArray[i] = data.truthArray[i];
            if (!truthArray[i]) continue;
            foreach (Transform toDeactive in transformsToDeactivate[i].transforms)
            {
                toDeactive.gameObject.SetActive(false);
            }
        }
    }
    public void SaveData(GameData data)
    {
        for (int i = 0; i < truthArray.Length; i++)
        {
            data.truthArray[i] = truthArray[i];
        }
    }

    [Serializable]
    private struct TransformsToDeactivate
    {
        [SerializeField] public Transform[] transforms;
    }
}

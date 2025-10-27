using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TruthArrayMaster : Singleton<TruthArrayMaster>, IDataPersistence
{
    [SerializeField] private TruthBehavior[] truthBehaviors;
    private bool[] truthArray;
    private TruthBehavior lastTruth;
    private void Awake()
    {
        InitializeSingleton();
        lastTruth.spawnPosition = Vector3.zero;
    }
    public void UpdateTruth(int idx)
    {
        if (idx < 0 && idx >= truthArray.Length)
        {
            Debug.LogError("Breh really?");
            return;
        }
        truthArray[idx] = true;
        lastTruth = truthBehaviors[idx];
        DataPersistenceManager.Instance.SaveGame();
    }
    public void LoadData(GameData data)
    {
        truthArray = new bool[data.truthArray.Length];
        for (int i = 0; i < data.truthArray.Length; i++)
        {
            truthArray[i] = data.truthArray[i];
            if (!truthArray[i]) continue;
            foreach (Transform toDeactive in truthBehaviors[i].transforms_to_deactivate)
            {
                toDeactive.gameObject.SetActive(false);
            }
            foreach (Transform toActive in truthBehaviors[i].transforms_to_activate)
            {
                toActive.gameObject.SetActive(true);
            }
        }
    }
    public void SaveData(GameData data)
    {
        for (int i = 0; i < truthArray.Length; i++)
        {
            data.truthArray[i] = truthArray[i];
        }
        if (lastTruth.spawnPosition == Vector3.zero) return;
        data.playerPosition[SceneManager.GetActiveScene().name] = lastTruth.spawnPosition;
    }

    [Serializable]
    private struct TruthBehavior
    {
        [SerializeField] public Vector3 spawnPosition;
        [SerializeField] public Transform[] transforms_to_deactivate;
        [SerializeField] public Transform[] transforms_to_activate;
    }
}

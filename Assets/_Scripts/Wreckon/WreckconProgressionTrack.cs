using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckconProgressionTrack : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject[] eventLocations;
    [SerializeField] private GameObject[] pawnGameObjects;
    public void LoadData(GameData data)
    {
        if (eventLocations.Length < 4) 
        {
            Debug.LogError("Please Add All Event Locations Scrub!");
            return;
        }
        if (data.truthArray[3]) 
        {
            Debug.LogError("HOW ARE YOU HERE? YOU SHOULDN'T BE HERE!");
            return;
        }
        int i = 0;
        while (data.truthArray[i])
        {
            eventLocations[i].SetActive(false);
            pawnGameObjects[i].SetActive(false);
            i++;
        }
    }

    public void SaveData(GameData data)
    {
        // No Need to do anything
    }
}

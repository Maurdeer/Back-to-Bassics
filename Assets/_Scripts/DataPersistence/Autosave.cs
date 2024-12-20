using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autosave : MonoBehaviour
{
    [SerializeField] private DataPersistenceManager data;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerPoncho")
        {
            DataPersistenceManager.instance.SaveGame();
            Debug.Log("Auto-saved Game");
        }
    }
}

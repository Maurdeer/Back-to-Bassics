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
            DataPersistenceManager.Instance.SaveGame();

            // Change to 
            // UIManager.Instance.SaveUI.Saving();
            // UIManager.Instance.SaveUI.SavingComplete();
            Debug.Log("Auto-saved Game");
        }
    }
}

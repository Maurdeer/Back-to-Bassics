using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WreckonTitle : MonoBehaviour
{
    private string m_saveProfileName;

    public void UpdateSaveProfile(string saveProfileName)
    {
        m_saveProfileName = saveProfileName;
    }

    public void StartGame()
    {
        if (m_saveProfileName.Trim() == "" || m_saveProfileName.Length > 30) 
        {
            Debug.LogError("Bad Name!");
            return;
        }
        DataPersistenceManager.instance.ChangeSelectedProfileId(m_saveProfileName);

        if (!DataPersistenceManager.instance.HasGameData())
        {
            DataPersistenceManager.instance.NewGame(GameDataIntialize.Wreckcon);
        }

        DataPersistenceManager.instance.LoadGame();
    }
}

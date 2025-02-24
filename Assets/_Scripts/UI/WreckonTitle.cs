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
        DataPersistenceManager.Instance.ChangeSelectedProfileId(m_saveProfileName);

        if (!DataPersistenceManager.Instance.HasGameData())
        {
            DataPersistenceManager.Instance.NewGame(GameDataIntialize.Wreckcon);
        }

        DataPersistenceManager.Instance.LoadGame();
    }
}

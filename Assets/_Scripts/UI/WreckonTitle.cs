using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WreckonTitle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_errorText;
    private string m_saveProfileName = "";

    public void UpdateSaveProfile(string saveProfileName)
    {
        m_saveProfileName = saveProfileName;
    }

    public void StartGame()
    {
        if (m_saveProfileName.Trim() == "")
        {
            m_errorText.text = "Please enter a name";
            return;
        }
        if (m_saveProfileName.Length > 30) 
        {
            m_errorText.text = "Name cannot be above 30 characters";
            return;
        }
        DataPersistenceManager.Instance.ChangeSelectedProfileId(m_saveProfileName);

        if (!DataPersistenceManager.Instance.HasGameData())
        {
            DataPersistenceManager.Instance.NewGame(GameDataIntialize.Default);
        }

        DataPersistenceManager.Instance.LoadGame();
    }
}

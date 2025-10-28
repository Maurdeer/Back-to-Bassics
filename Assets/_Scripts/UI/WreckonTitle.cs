using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WreckonTitle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_errorText;
    [SerializeField] private TMP_Dropdown m_profileOptions;
    private string m_saveProfileName = "";
    public string SaveProfileName
    {
        get
        {
            return m_saveProfileName;
        }
        set
        {
            m_saveProfileName = value;
        }
    }
    private void Awake()
    {
        Dictionary<string, GameData> profiles = DataPersistenceManager.Instance.GetAllProfilesGameData();
        GameData[] datas = profiles.Values.ToArray();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (GameData data in datas)
        {
            options.Add(new TMP_Dropdown.OptionData(data.profileName));
        }
        m_profileOptions.AddOptions(options);
    }
    public void CreateNewProfile()
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
        if (DataPersistenceManager.Instance.GetAllProfilesGameData().ContainsKey(m_saveProfileName))
        {
            m_profileOptions.value = m_profileOptions.options.FindIndex(0, (TMP_Dropdown.OptionData optionData) => optionData.text == m_saveProfileName);
        }
        else
        {
            m_profileOptions.options.Add(new TMP_Dropdown.OptionData(m_saveProfileName));
            m_profileOptions.options = m_profileOptions.options;
            m_profileOptions.value = m_profileOptions.options.Count() - 1;
        }
        
        
    }
    public void UpdateOption(int option_idx)
    {
        m_saveProfileName = m_profileOptions.options[option_idx].text;
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

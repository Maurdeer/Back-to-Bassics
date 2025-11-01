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
    private Animator m_animator;
    private string m_saveProfileName = "";
    private bool menuShown = false;
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
        m_animator = GetComponent<Animator>();  
    }
    private void Start()
    {
        Dictionary<string, GameData> profiles = DataPersistenceManager.Instance.GetAllProfilesGameData();
        GameData[] datas = profiles.Values.ToArray();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (GameData data in datas)
        {
            options.Add(new TMP_Dropdown.OptionData(data.profileName));
        }
        m_profileOptions.AddOptions(options);
        UpdateOption(0);
    }
    private void Update()
    {
        if (Input.anyKey && !menuShown)
        {
            ShowMenu();
        }
    }
    public void ShowMenu()
    {
        menuShown = true;
        m_animator.Play("LogoGoUp");
    }
    public void CreateNewProfile()
    {
        if (DataPersistenceManager.Instance.GetAllProfilesGameData().ContainsKey(m_saveProfileName))
        {
            m_errorText.text = "Profile already exists, make a new one!";
            return;
        }
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
        m_profileOptions.options.Add(new TMP_Dropdown.OptionData(m_saveProfileName));
        m_profileOptions.options = m_profileOptions.options;
        m_profileOptions.value = m_profileOptions.options.Count() - 1;

        StartGame();
    }
    public void UpdateOption(int option_idx)
    {
        if (option_idx >= m_profileOptions.options.Count) return;
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

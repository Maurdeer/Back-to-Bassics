using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    private void Awake()
    {
        InitializeSingleton();
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        InitializeSelectedProfileId();

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        disableDataPersistence = false;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        disableDataPersistence = true;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // note that before loading a new scene you need to call SaveGame
        // to ensure that all the data is saved.
        // Using OnSceneUnloaded doesn't work because it tries to save
        // after all the objects in the scene are destroyed
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (scene.name.ToLower().Contains("title")) return;
        UpdatePersistentData();

    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        this.selectedProfileId = newProfileId;
        // load the game, which will use that profile, updating our game data accordingly
        //LoadGame();

        // load any saved data from a file via the datahandler
        this.gameData = dataHandler.Load(selectedProfileId);

        // if it can't find any data to load, don't continue
        if (this.gameData == null)
        {
            Debug.Log("No game data found! A New Game needs to be started before data can be loaded.");
            return;
        }
    }

    public void DeleteProfileData(string profileId)
    {
        // delete the data for this profile id
        dataHandler.Delete(profileId);
        // initialize the selected profile id
        InitializeSelectedProfileId();
        // reload the game so that our data matches the newly selected profile id
        //LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        ChangeSelectedProfileId(dataHandler.GetMostRecentlyUpdatedProfileId());
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    public void NewGame(GameDataIntialize initialize = GameDataIntialize.Default)
    {
        //Debug.Log("Initializing new game...");
        this.gameData = new GameData(selectedProfileId, initialize);

        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        dataHandler.Save(gameData, selectedProfileId);
    }

    public void LoadGame()
    {
        //Debug.Log("Loading game...");

        // return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        // Change scenes to fully load data
        SceneManagement.Instance.ChangeScene(this.gameData.currentScene);
    }

    public void UpdatePersistentData()
    {
        // Should be loaded hopefully
        if (this.gameData == null) return;

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(this.gameData);
        }
    }

    public void SaveGame()
    {
        // return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        if (this.gameData == null)
        {
            Debug.LogWarning("No game data found! A New Game needs to be started before data can be saved.");
            return;
        }

        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            try
            {
                dataPersistenceObj.SaveData(gameData);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to save data: {e}");
            }
        }

        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // save that data to a file using the datahandler
        dataHandler.Save(gameData, selectedProfileId);
    }

    //called whenever the game quits
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        //search for all IDataPersistence objects, including inactive ones
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

}

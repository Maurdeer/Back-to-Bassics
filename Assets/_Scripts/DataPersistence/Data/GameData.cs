using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string profileName;
    public long lastUpdated;
    public string currentScene;
    public SerializableDictionary<string, Vector3> playerPosition;
    public SerializableDictionary<string, Combo> combosUnlocked;

    // Example Event Bools
    public bool[] truthArray;

    //example of how to save a dictionary; potentially useful for collectibles
    //and even which bosses have been defeated. Make sure to give each a unique
    //string to use as its ID/key in the dictionary
    //public SerializableDictionary<string, bool> bossesDefeated;
    //public SerializableDictionary<string, bool> cutscenesPlayed;

    // Achievements
    public bool[] wreckconQuests;
    public ulong[] enemyScore;

    //values defined in this constructor are the values each save starts with
    public GameData(string profileName, GameDataIntialize initialData = GameDataIntialize.Default)
    {
        this.profileName = profileName;
        switch (initialData)
        {
            case GameDataIntialize.Wreckcon:
                Wreckcon();
                break;
            default:
                Default();
                break;
        } 
    }
    private void Wreckcon()
    {
        playerPosition = new SerializableDictionary<string, Vector3>();
        playerPosition.Add("test", new Vector3(829.789978f, 100.028999f, 238.210007f));
        currentScene = "test";
        combosUnlocked = new SerializableDictionary<string, Combo>();

        // Event Pools Set
        truthArray = new bool[7];
        wreckconQuests = new bool[16];
        enemyScore = new ulong[4];
    }
    private void Default()
    {
        //Set player position at the start of the game
        playerPosition = new SerializableDictionary<string, Vector3>();
        playerPosition.Add("BaselineShores", new Vector3(330.4f, 20, 79.89f));
        currentScene = "BaselineShores";
        combosUnlocked = new SerializableDictionary<string, Combo>();

        // Event Pools Set
        truthArray = new bool[7];

        //Debug.Log("New save created.");
    }
}

public enum GameDataIntialize
{
    Default=0,
    Wreckcon
}

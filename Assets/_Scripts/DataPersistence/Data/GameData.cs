using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public Vector3 playerPosition;

    //example of how to save a dictionary; potentially useful for collectibles
    //and even which bosses have been defeated. Make sure to give each a unique
    //string to use as its ID/key in the dictionary
    //public SerializableDictionary<string, bool> bossesDefeated;

    //values defined in this constructor are the values each save starts with
    public GameData()
    {
        //Set player position at the start of the game
        this.playerPosition = new Vector3(302, 20, 90);
        //bossesDefeated = new SerializableDictionary<string, bool>();
        Debug.Log("New save created.");
    }
}

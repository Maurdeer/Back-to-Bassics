using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    //values defined in this constructor are the values each save starts with
    public GameData()
    {
        //Set player position at the start of the game
        this.playerPosition = new Vector3(302, 20, 90);

        //Set player xyz rotation at the start of the game
        this.playerRotation = new Quaternion(0, 0, 0, 0);
    }
}

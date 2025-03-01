using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HighScore : MonoBehaviour
{
    [SerializeField] private Transform highscoreRegion;
    [SerializeField] private TMP_FontAsset fontAsset;
    public class HighScoreComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            GameData gameDataX = (GameData)x;
            GameData gameDataY = (GameData)y;
            return gameDataX.totalScore > gameDataY.totalScore ? -1 : (gameDataX.totalScore < gameDataY.totalScore ? 1 : 0);
        }
    }
    private void Start()
    {
        Dictionary<string, GameData> profiles = DataPersistenceManager.Instance.GetAllProfilesGameData();
        GameData[] profileGamedata = profiles.Values.ToArray();
        Array.Sort(profileGamedata, new HighScoreComparer());

        for (int i = 0; i < 10; i++)
        {
            if (i > profileGamedata.Length)
            {
                return;
            }
            GameObject highScoreObject = new GameObject();
            TextMeshProUGUI mesh = highScoreObject.AddComponent<TextMeshProUGUI>();
            mesh.text = $"{profileGamedata[i].profileName} | {profileGamedata[i].totalScore}";
            mesh.font = fontAsset;
            highScoreObject.transform.SetParent(highscoreRegion, false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M4Start : MonoBehaviour
{
    private static M4Start instance;
    private GameObject poncho;
    // 1 - gotStick, 2 - bassicsBeaten, 3 - smallFryBeaten, 4 - turboTopMeet, 5 - turboTopBeaten, 6 - kingSalMeet, 7 - kingSalBeaten;
    [SerializeField] private bool[] truthArray = new bool[7];
    [SerializeField] private Combo sizzle;  
    // [SerializeField] public bool gotStick;
    // [SerializeField] public bool bassicsBeaten;
    // [SerializeField] public bool smallFryBeaten;
    // [SerializeField] public bool turboTopMeet;
    // [SerializeField] public bool turboTopBeaten;
    // [SerializeField] public bool kingSalMeet;
    // [SerializeField] public bool kingSalBeaten;

                            

    void Awake() {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                return;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }     
    }
    private int idx;
    public void SetTruthIndex(int idx)
    {
        this.idx = idx;
    }
    public void SetBool(bool value)
    {
        truthArray[idx] = value;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        poncho = GameObject.Find("PlayerPoncho");
        // hacky but so that we don't do any questionable things
        if (scene.name == "Title")
        {  
            return;
        }
        Debug.Log("M4 Start Ready to Go!");
        if (truthArray[0]) {
            Debug.Log("Stick confirmed to have been picked up!");

            GameObject introCutscene = GameObject.Find("IntroCutscene");
            if (introCutscene != null) {
                Debug.Log("Found the intro cutscene and disabling it!");
                introCutscene.SetActive(true);
            } else {
                Debug.Log("Didn't find an intro cutscene. That's strange...");
            }
            // GameManager.Instance.GSM.Transition<GameStateMachie.WorldTraversal>();
            GameObject stick = GameObject.Find("Interactable");
            if (stick != null) {
                Debug.Log("Found the stick!");
                stick.transform.position = poncho.transform.position;
            } else {
                Debug.Log("Didn't find the stick. That's strange...");
            }
            GameObject fry1 = GameObject.Find("Fry1");
            if (fry1 != null) {
                Debug.Log("Found Small Fry's first set of quests! Disabled them.");
                fry1.SetActive(false);
            } else {
                Debug.Log("Couldn't find Small Fry's first set of things. That's strange...");
            }
            GameObject bassicsScene1 = GameObject.Find("BassicsIntro (1)");
            GameObject bassicsScene2 = GameObject.Find("BassicsIntro (3)");
            Debug.Log(bassicsScene1.name);
            Debug.Log(bassicsScene2.name);
            if (bassicsScene1 != null) {
                Debug.Log("Found the first scene and disabled it!");
                bassicsScene1.SetActive(false);
            }
            if (bassicsScene2 != null) {
                Debug.Log("Found the second scene and enabled it!");
                bassicsScene2.SetActive(true);
            }
        }
        if (truthArray[1]) {
            GameObject bassicsFightTrigger = GameObject.Find("BassicIntro Triggers");
            if (bassicsFightTrigger)
            {
                bassicsFightTrigger?.SetActive(false);
                poncho.transform.position = bassicsFightTrigger.transform.position;
            }
            
            GameObject.Find("BassicsPostFight (1)")?.SetActive(true);
        }
        if (truthArray[2]) {
            poncho.GetComponent<ComboManager>().AddCombo(sizzle);
            GameObject smallFry = GameObject.Find("SmallFryPawn");
            smallFry.transform.position = GameObject.Find("SmallFryDialogue1Location").transform.position;
            GameObject.Find("SmallFryPost")?.SetActive(true);
            GameObject.Find("FriedRice")?.SetActive(false);
        }
        if (truthArray[3]) {
            GameObject.Find("TorchPuzzleCube")?.SetActive(false);
            GameObject.Find("SmallFryPost2")?.SetActive(false);
            GameObject.Find("TurboTopPawn")?.SetActive(true);
        }
        if (truthArray[4]) {
            GameObject.Find("TurboTopPost")?.SetActive(true);
            GameObject.Find("KingSalPawn")?.SetActive(true);
            GameObject.Find("FightKingSal")?.SetActive(true);
        }
    }

    public void statusUpdate(int n) {
        truthArray[n] = true;
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}

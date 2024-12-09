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
        if (truthArray[0]) {
            GameObject.Find("IntroCutscene")?.SetActive(false);
            // GameManager.Instance.GSM.Transition<GameStateMachie.WorldTraversal>();
            GameObject.Find("Interactable").transform.position = poncho.transform.position;
            GameObject.Find("Fry1")?.SetActive(false);
            GameObject.Find("BassicsIntro (1)")?.SetActive(false);
        }
        if (truthArray[1]) {
            GameObject bassicsFightTrigger = GameObject.Find("BassicIntroTriggers");
            if (bassicsFightTrigger)
            {
                bassicsFightTrigger?.SetActive(false);
                poncho.transform.position = bassicsFightTrigger.transform.position;
            }
            
            GameObject.Find("BassicsPostFight(1)")?.SetActive(true);
        }
        if (truthArray[2]) {
            poncho.GetComponent<ComboManager>().AddCombo(sizzle);
        }
        if (truthArray[3]) {
            GameObject.Find("TorchPuzzleCube")?.SetActive(false);
            GameObject.Find("SmallFryPost2")?.SetActive(false);
        }
        if (truthArray[4]) {
            GameObject.Find("TurboChillTop")?.SetActive(true);
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

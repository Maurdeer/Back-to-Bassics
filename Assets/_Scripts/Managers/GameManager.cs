using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static GameStateMachine;

public class GameManager : Singleton<GameManager>
{
    public GameStateMachine GSM { get; private set; }
    public PlayerController PC { get; private set; }
    public PlayableDirector Director { get; private set; }

    private void Awake()
    {
        InitializeSingleton();

        // References
        GSM = GetComponent<GameStateMachine>();
        PC = FindObjectOfType<PlayerController>();
        Director = GetComponent<PlayableDirector>();
    }

    public void PlayCutscene(PlayableDirector director)
    {
        GSM.Transition<Cutscene>();
        director.Play();
    }
    public void PlayCutscene(TimelineAsset asset)
    {
        GSM.Transition<Cutscene>();
        Director.playableAsset = asset;
        Director.Play();
    }
    public void SaveGame()
    {
        DataPersistenceManager.Instance.SaveGame();
    }
}

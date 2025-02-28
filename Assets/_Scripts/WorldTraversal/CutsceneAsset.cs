using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static GameStateMachine;

public class CutsceneAsset : MonoBehaviour
{
    [SerializeField] private bool playOnAwake;
    [SerializeField] private StateAfterCutscene nextState;
    private PlayableDirector director;
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        
    }

    private void Start()
    {
        if (playOnAwake) PlayCutscene();
    }

    public void PlayCutscene()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        director.Play();
        GameManager.Instance.GSM.Transition<Cutscene>();
        StartCoroutine(ReturnToPreviousState());
    }

    private IEnumerator ReturnToPreviousState()
    {
        yield return new WaitUntil(() => director.state != PlayState.Playing);
        GameManager.Instance.GSM.PrevState.GetType();

        switch (nextState)
        {
            case StateAfterCutscene.traversal:
                GameManager.Instance.GSM.Transition<WorldTraversal>();
                break;
            case StateAfterCutscene.battle:
                GameManager.Instance.GSM.Transition<Battle>();
                break;
            case StateAfterCutscene.dialogue:
                GameManager.Instance.GSM.Transition<Dialogue>();
                break;
        } 
    }
}

public enum StateAfterCutscene
{
    traversal,
    battle,
    dialogue
}
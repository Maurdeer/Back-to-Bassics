using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] EnemyBattlePawn _enemyBattlePawn;
    [SerializeField] GameObject comboList;
    private void Awake()
    {
        bool wasInitialized = InitializeSingleton(this);
        if (!wasInitialized)
        {
            Debug.LogError("Tutorial Manager Already Initialized!");
        }
    }

    void Start()
    {
        _enemyBattlePawn.OnEnemyStaggerEvent += ShowComboList;
        _enemyBattlePawn.OnEnemyUnstaggerEvent += HideComboList;
    }

    public void ShowComboList()
    {
        comboList.SetActive(true);
    }

    public void HideComboList()
    {
        comboList.SetActive(true);
    }
    
    public void Pause()
    {
        Conductor.Instance.PauseCondcutor();
        _enemyBattlePawn.PawnSprite.Animator.speed = 0f;
    }

    public void Unpause()
    {
        Conductor.Instance.ResumeConductor();
        _enemyBattlePawn.PawnSprite.Animator.speed = 1f;
        DialogueManager.Instance.customDialogueRunner.Stop();
    }

}

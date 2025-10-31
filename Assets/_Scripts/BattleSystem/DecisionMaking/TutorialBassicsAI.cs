using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static EnemyStateMachine;
using static PositionStateMachine;

public class TutorialBassicsAI : BossAI
{
    // [SerializeField] protected UnityEvent firstTimeStagger;
    // [SerializeField] protected UnityEvent secondTimeStagger;
    private bool staggeredBefore = false;
    // (Joseph 1 / 11 / 2025) Was running into a stack overflow issue when calling firstStagger through checking enemy stages and making sure it was the first one.
    // I'm going to try to make it so it's only called every beat, and if 
    // (Joseph 10/19/2025) What was I talking about, I have no clue.

    protected override void Start()
    {
        base.Start();
        _enemyBattlePawn.OnEnemyStaggerEvent += delegate
        {
            // (Joseph 1 / 11 / 2025) Modifying this to account for Bassic's behavior in the tutorial
            // (10/14/2025 Joseph) No need to repeat dialogue, as the dialogue should persist during the tutorial until the player advances. 
            if (_currentStage == 0 && !staggeredBefore)
            {
                staggeredBefore = true;
                _enemyBattlePawn.StaggerFor(999);
                DialogueManager.Instance.RunDialogueNode("bassics-battle_first-stagger");
            }
            if (_currentStage == 1 && !staggeredBefore)
            {
                staggeredBefore = true;
                DialogueManager.Instance.RunDialogueNode("bassics-battle_second-stagger");
            }
        };
        _enemyBattlePawn.OnEnemyUnstaggerEvent += delegate
        {
            // Modify this by so much HOLY SHIT
            // For Tutorial Game Feel
            if (_currentStage == 1 && staggeredBefore)
            {
                DialogueManager.Instance.RunDialogueNode("bassics-battle_first-unstagger");
            }
        };
        OnEnemyStageTransition += delegate
        {
            staggeredBefore = false;
        };
    }



}


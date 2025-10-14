using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static EnemyStateMachine;

public class KingSalAI : BossAI
{
    [Header("King Sal Specific Parameters")]
    [SerializeField] private SummonSubortinate regularSubordinateSummon;
    [SerializeField] private int defaultSalSummonProbability;
    [SerializeField] private int increaseSummonProbabilityAfterFailure;
    private int salSummonProbability;
    // private int summonIdx = 1;

    private void Start() {
        base.Start();
        _enemyBattlePawn.OnPawnDeath += CheckAchievementOnDeath;
        salSummonProbability = defaultSalSummonProbability;
    }
    
    protected override EnemyAttackPattern MakeDecision()
    {
        EnemyAttackPattern[] actions = _enemyStages[_currentStage].EnemyAttackPatterns;

        if (_currentStage < 1)
        { // Phase 1 for King Sal, just choose randomly
            idx = Random.Range(0, actions != null ? actions.Length : 0);
            if (idx == _lastAction) idx = (actions.Length - idx) % actions.Length;

        }
        else if (_currentStage >= 1)
        { // Phase 2 and 3 for King Sal
            if (!regularSubordinateSummon.GetIsFull())
            {
                if (Random.Range(1, 101) <= salSummonProbability)
                {
                    idx = 0; // Range is 1 - 100 I believe
                    salSummonProbability = defaultSalSummonProbability;
                }
                else
                {
                    salSummonProbability += increaseSummonProbabilityAfterFailure;
                    idx = Random.Range(1, actions != null ? actions.Length : 0);
                    if (idx == _lastAction) idx = (actions.Length - idx) % (actions.Length);
                    // Debug.Log("Sal Summon Probability right now is " + salSummonProbability);


                }
            }
            else
            {
                idx = Random.Range(1, actions != null ? actions.Length : 0);
                if (idx == _lastAction) idx = (actions.Length - idx) % actions.Length;
            }
        }
        return actions[idx];
    }

    private void CheckAchievementOnDeath() {
        if (regularSubordinateSummon.CompletedKillNoSubordinatesTask()) UIManager.Instance.WreckconQuests.MarkAchievement(13);
    }


}

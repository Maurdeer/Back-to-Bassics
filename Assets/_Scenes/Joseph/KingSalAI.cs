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
    private bool empoweredSubordinates = false;
    // private int summonIdx = 1;

    private void Start() {
        base.Start();
        _enemyBattlePawn.OnPawnDeath += CheckAchievementOnDeath;
    }
    protected override void PhaseChange() {
        base.PhaseChange();
        if (!empoweredSubordinates && _currentStage == 2) {
            regularSubordinateSummon.UpgradeMinions();
            empoweredSubordinates = true;
        }
    }

    protected override void OnFullBeat() {
        // (Ryan) Should't need to check for death here, just disable the conducatable conductor connection
        if (Conductor.Instance.Beat <= 1)
        {
            OnFirstBeat();
        }
        PhaseChange();

        // (Joseph 1 / 11 / 2025) Was running into a stack overflow issue when calling firstStagger through checking enemy stages and making sure it was the first one.
        // I'm going to try to make it so it's only called every beat, and if 
        staggeredBefore = false;

        if (_enemyBattlePawn.Director.state == PlayState.Playing 
            || _enemyBattlePawn.IsDead || _enemyBattlePawn.IsStaggered || DialogueManager.Instance.IsDialogueRunning) return;
        
        if (_decisionTime > 0) {
            // counting down time between attacks
            _decisionTime--;
            return;
        }
            
        EnemyAttackPattern[] actions = _enemyStages[_currentStage].EnemyAttackPatterns;
        
        // Summon is indexed at 0, always. For DreamHack at least. 
        int idx = 1;
        if (!regularSubordinateSummon.GetIsFull() && _lastAction != 0) idx = 0;
        else {
            idx = Random.Range(regularSubordinateSummon.GetIsFull()? 1 : 0, actions != null ? actions.Length : 0);
            if (idx == _lastAction) idx = (actions.Length - idx) % actions.Length;
        }
        _lastAction = idx;

        // may want to abstract enemy actions away from just timelines in the future?
        _enemyBattlePawn.interruptable = actions[idx].Interruptable;
        // Reset Stagger Health Only on Stage Request!
        if (_enemyStages[_currentStage].ResetStaggerHealth)
            _enemyBattlePawn.currentStaggerHealth = _enemyBattlePawn.maxStaggerHealth;
        _enemyBattlePawn.esm.Transition<Attacking>();
        _enemyBattlePawn.Director.playableAsset = actions[idx].ActionSequence;
        _enemyBattlePawn.Director.Play();
        var handle = _enemyBattlePawn.Director.ScheduleToBeat();

        _decisionTime = _beatsPerDecision;
    }

    private void CheckAchievementOnDeath() {
        if (regularSubordinateSummon.CompletedKillNoSubordinatesTask()) UIManager.Instance.WreckconQuests.MarkAchievement(13);
    }


}

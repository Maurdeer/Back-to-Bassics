using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public bool IsBattleActive { get; private set; } 
    public PlayerBattlePawn Player { get; set; }
    public EnemyBattlePawn Enemy { get; set; }
    private float battleDelay = 3f;
    private Queue<EnemyBattlePawn> enemyBattlePawns;
    private void Awake()
    {
        InitializeSingleton();
    }
    public void Start()
    {
        Player = GameManager.Instance.PC.GetComponent<PlayerBattlePawn>();
    }
    public void StartBattle(EnemyBattlePawn[] pawns)
    {
        enemyBattlePawns = new Queue<EnemyBattlePawn>(pawns);
        Enemy = enemyBattlePawns?.Dequeue();
        if (Enemy == null)
        {
            Debug.LogError("BattleManager tried to start battle, but player has no Enemy Opponent!");
            return;
        }
        StartCoroutine(IntializeBattle());
    }
    public void EndBattle()
    {
        IsBattleActive = false;
        Conductor.Instance.StopConducting();
        Player.ExitBattle();
        Enemy.ExitBattle();
        // Instead of directly to world traversal, need a win screen of some kind
        GameManager.Instance.GSM.Transition<GameStateMachine.WorldTraversal>();
    }
    private IEnumerator IntializeBattle()
    {
        GameManager.Instance.PC.DisableControl();
        yield return PlayerEngageCurrentEnemy();
        for (float i = battleDelay; i > 0; i--)
        {
            UIManager.Instance.UpdateCenterText(i.ToString());
            yield return new WaitForSeconds(1f);
        }
        UIManager.Instance.UpdateCenterText("Battle!");
        Conductor.Instance.BeginConducting(Enemy.EnemyData.SPB);
        GameManager.Instance.GSM.Transition<GameStateMachine.Battle>();
        Player.EnterBattle();
        Enemy.EnterBattle();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.UpdateCenterText("");
        IsBattleActive = true;
    }
    private IEnumerator NextEnemyBattle()
    {
        // The problem with this is that the player can still input stuff while transitioning.
        yield return PlayerEngageCurrentEnemy();
        Enemy.EnterBattle();
        //for (float i = battleDelay; i > 0; i--)
        //{
        //    UIManager.Instance.UpdateCenterText(i.ToString());
        //    yield return new WaitForSeconds(1f);
        //}
        UIManager.Instance.UpdateCenterText("Battle!");
        yield return new WaitForSeconds(1f);
        UIManager.Instance.UpdateCenterText("");
        Conductor.Instance.BeginConducting(Enemy.EnemyData.SPB);
        IsBattleActive = true;
    }
    public void OnPawnDeath(BattlePawn pawn) 
    {
        if (pawn is PlayerBattlePawn) 
        {
            OnPlayerDeath();
        }
        else if (pawn is EnemyBattlePawn) 
        {
            OnEnemyDeath();
        }
    }
    private void OnPlayerDeath()
    {
        EndBattle();
        //UIManager.Instance.UpdateCenterText("Player Is Dead, SAD!");
        GameManager.Instance.GSM.Transition<GameStateMachine.Death>();
    }
    private void OnEnemyDeath()
    {
        if (enemyBattlePawns.Count > 0)
        {
            // TODO: Multiple Enemy Logic
            Enemy.ExitBattle();
            Enemy = enemyBattlePawns.Dequeue();
            StartCoroutine(NextEnemyBattle());
            return;
        }
        EndBattle();

        // For now we won't use this
        // StartCoroutine(EnemyDefeatTemp());
    } 

    private IEnumerator EnemyDefeatTemp()
    {
        UIManager.Instance.UpdateCenterText($"Defeated {Enemy.Data.Name}!");
        yield return new WaitForSeconds(3f);
        UIManager.Instance.UpdateCenterText("");
    }
    private IEnumerator PlayerEngageCurrentEnemy()
    {
        TraversalPawn traversalPawn = Player.GetComponent<TraversalPawn>();
        traversalPawn.MoveToDestination(Enemy.targetFightingLocation.position);
        yield return new WaitUntil(() => !traversalPawn.movingToDestination);
    }
}

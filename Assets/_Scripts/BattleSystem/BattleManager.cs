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
    private ulong m_playerScore;
    public ulong PlayerScore
    {
        get { return m_playerScore; }
        set
        {
            m_playerScore = value;
            UIManager.Instance.ScoreTracker.UpdateScore(m_playerScore);
        }
    }
    private uint m_playerMultiplier;
    public uint PlayerMultiplier
    {
        get { return m_playerMultiplier; }
        set
        {
            m_playerMultiplier = value;
            UIManager.Instance.ScoreTracker.UpdateMultiplier(m_playerMultiplier);
        }
    }
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
        PlayerScore = 0;
        PlayerMultiplier = 1;   
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
        UIManager.Instance.ClockUI.StopClock();
        Conductor.Instance.StopConducting();
        CalculateAndUpdateScore();
        GameManager.Instance.GSM.Transition<GameStateMachine.WorldTraversal>();
        Player.ExitBattle();
        Enemy.ExitBattle();
        // Instead of directly to world traversal, need a win screen of some kind
    }
    private IEnumerator IntializeBattle()
    {
        GameManager.Instance.PC.DisableControl();
        yield return PlayerEngageCurrentEnemy();
        yield return Enemy.PlayIntroCutscene();

        // Reset Player Health and Combo
        Player.Heal(Player.MaxHP);
        Player.ComboManager.CurrComboMeterAmount = 0;

        Player.EnterBattle();
        Enemy.EnterBattle();
        AudioManager.Instance.SetAmbienceVolume(0.1f);
        CameraConfigure.Instance.SwitchToCamera(Enemy.battleCam);
        for (float i = battleDelay; i > 0; i--)
        {
            UIManager.Instance.UpdateCenterText(i.ToString());
            yield return new WaitForSeconds(1f);
        }
        UIManager.Instance.UpdateCenterText("Battle!");
        UIManager.Instance.ClockUI.StartClock();
        UIManager.Instance.ScoreTracker.StartTimeMultiplier(Enemy.EnemyData.ClockDecayTH);
        Conductor.Instance.BeginConducting(Enemy);
        GameManager.Instance.GSM.Transition<GameStateMachine.Battle>();
        Player.StartBattle();
        Enemy.StartBattle();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.UpdateCenterText("");
        IsBattleActive = true;
    }
    private IEnumerator NextEnemyBattle()
    {
        // The problem with this is that the player can still input stuff while transitioning.
        yield return PlayerEngageCurrentEnemy();
        Enemy.EnterBattle();
        CameraConfigure.Instance.SwitchToCamera(Enemy.battleCam);
        //for (float i = battleDelay; i > 0; i--)
        //{
        //    UIManager.Instance.UpdateCenterText(i.ToString());
        //    yield return new WaitForSeconds(1f);
        //}
        UIManager.Instance.UpdateCenterText("Battle!");
        yield return new WaitForSeconds(1f);
        Enemy.StartBattle();
        UIManager.Instance.UpdateCenterText("");
        Conductor.Instance.BeginConducting(Enemy);
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
        CameraConfigure.Instance.SwitchBackToPrev();
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
    public void AddPlayerScore(ulong score)
    {
        PlayerScore += score * PlayerMultiplier;
    }
    public void AddPlayerMultiplier(uint multiplier)
    {
        PlayerMultiplier += multiplier;
    }
    public void ResetPlayerMultiplier()
    {
        PlayerMultiplier = 1;
    }
    private void CalculateAndUpdateScore()
    {
        float timeMultiplier = UIManager.Instance.ScoreTracker.StopAndGetTimeMultiplier();
        // Kill Ryan For Hardcodeness NOW!
        int id = Enemy.Data.name == "Bassics" ? 0 : (Enemy.Data.name == "SmallFry" ? 1 : (Enemy.Data.name == "TurboTop" ? 2 : (Enemy.Data.name == "KingSal" ? 3 : -1)));
        ulong finalScore = (ulong)(timeMultiplier * m_playerScore);
        UIManager.Instance.PersistentDataTracker.UpdateEnemyScore(id, finalScore);
    }
}

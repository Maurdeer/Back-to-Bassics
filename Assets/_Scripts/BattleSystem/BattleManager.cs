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
    private readonly string[] ranks = {"S", "A", "B", "C", "D"};
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
        GameManager.Instance.GSM.Transition<GameStateMachine.UIState>();
        UIManager.Instance.ClockUI.StopClock();
        Conductor.Instance.StopConducting();
    }
    
    public void EndBattleComplete()
    {
        GameManager.Instance.GSM.Transition<GameStateMachine.WorldTraversal>();
        Player.ExitBattle();
        Enemy.ExitBattle();  
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
        UIManager.Instance.EnemyIcon.sprite = Enemy.EnemyData.Icon;
        for (float i = battleDelay; i > 0; i--)
        {
            UIManager.Instance.UpdateCenterText(i.ToString());
            yield return new WaitForSeconds(1f);
        }
        UIManager.Instance.UpdateCenterText("Battle!");
        UIManager.Instance.ClockUI.StartClock();
        UIManager.Instance.ScoreTracker.StartTimeMultiplier(Enemy.EnemyData.ClockDelayTH, Enemy.EnemyData.ClockDecayTH);
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
        Player.ExitBattle();
        Enemy.ExitBattle();

        // Lose Logic
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

        // Victory Logic
        // Update Score: Kill Ryan For Hardcodeness NOW!
        int id = EnemyId(Enemy.EnemyData.Name);
        ulong finalScore = UIManager.Instance.ScoreTracker.StopAndGetFinalScore();
        UIManager.Instance.WreckconQuests.MarkAchievement(id * 4);
        // Rank Calculation
        double scoreFraction = (double)finalScore / Enemy.EnemyData.SRankMax;
        string scoreRank = "";
        double fraction = 1;
        foreach (string rank in ranks)
        {
            if (scoreFraction >= fraction)
            {
                scoreRank = rank;
                if (scoreRank == "S")
                {
                    UIManager.Instance.WreckconQuests.MarkAchievement(id * 4 + 2);
                    UIManager.Instance.WreckconQuests.MarkAchievement(id * 4 + 3);
                }
                else if (scoreRank == "A")
                {
                    UIManager.Instance.WreckconQuests.MarkAchievement(id * 4 + 3);
                }

                break;
            }
            fraction -= 0.2;
        }

        if (scoreRank == "") scoreRank = "E";

        UIManager.Instance.PersistentDataTracker.UpdateEnemyScore(id, finalScore, scoreRank);

        UIManager.Instance.BeatEnemyPanel.PlayBattleVictory(Enemy.EnemyData.Name, finalScore, Enemy.EnemyData.SRankMax, scoreRank);
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
    private int EnemyId(string name)
    {
        return name == "Bassics" ? 0 : (name == "Small Fry" ? 1 : (name == "Turbo Top" ? 2 : (name == "King Sal" ? 3 : -1)));
    }
}

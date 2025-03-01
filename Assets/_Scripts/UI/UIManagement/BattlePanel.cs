using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Rewrite this trash, its ugly to read
/// </summary>
public partial class UIManager
{
    [Header("Battle Panel")]
    [SerializeField] private TextMeshProUGUI parryTracker;
    [SerializeField] private TextMeshProUGUI blockTracker;
    [SerializeField] private TextMeshProUGUI missTracker;
    [SerializeField] private TextMeshProUGUI beatTracker;
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private Animator battlePanelAnimator;
    [SerializeField] private HealthBar _playerHpBar;
    [SerializeField] private Image _enemyHpBar;
    [SerializeField] private TextMeshProUGUI _enemyHpText;
    [SerializeField] private ComboGaugeBar _comboGaugeBar;
    public BeatEnemyPanel BeatEnemyPanel;
    public BeatIndicator BeatIndicator;
    public Image EnemyIcon;
    public ClockUI ClockUI;
    public ScoreTracker ScoreTracker;
    public ComboDisplay ComboDisplay;
    public int parryCount;
    public int blockCount;
    public int missCount;
    // Replacement For easier use maybe
    //public GaugeTracker PlayerHP;
    //public GaugeTracker EnemyHP;
    //public GaugeTracker ComboMeter;
    // Debug
    private void FixedUpdate()
    {
        if (Conductor.Instance.IsPlaying)
        {
            beatTracker.text = $"Beat: {(int)Conductor.Instance.Beat}";
        }
        else
        {
            beatTracker.text = "";
        }
    }
    public void IncrementParryTracker()
    {
        parryTracker.text = $"Parries: {++parryCount}";
    }
    public void IncrementBlockTracker()
    {
        blockTracker.text = $"Blocks: {++blockCount}";
    }
    public void IncrementMissTracker()
    {
        missTracker.text = $"Misses: {++missCount}";
    }
    //---------
    public void UpdateCenterText(string text)
    {
        centerText.text = text;
    }
    public void UpdateHP(BattlePawn pawn)
    {     
        if (pawn is PlayerBattlePawn)
        {
            // Player Pawn
            _playerHpBar.UpdateHealth(pawn.HP, pawn.Data.HP);
            return;
        }

        // Enemy Pawn
        _enemyHpBar.fillAmount = (float)pawn.HP / pawn.Data.HP;
        _enemyHpText.text = $"{pawn.HP}/{pawn.Data.HP}";
    }
    public void SetPlayerHealthBarOnFire()
    {
        _playerHpBar.SetOnFire();
    }
    public void ExtinguishPlayerHealthBarOnFire()
    {
        _playerHpBar.ExstinguishFire();
    }
    public void UpdateComboMeter(ComboManager manager)
    {
        _comboGaugeBar.UpdateGauge(manager.CurrComboMeterAmount, manager.MaxComboMeterAmount);
    }
    public void ShowBattlePanel()
    {
        battlePanelAnimator.Play("ShowBattlePanel");
    }
    public void HideBattlePanel()
    {
        battlePanelAnimator.Play("HideBattlePanel");
    }
}

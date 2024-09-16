using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyBattle : EnemyBattlePawn
{
    private ComboManager _playerComboManager;
    private void Start()
    {
        _playerComboManager = BattleManager.Instance.Player.GetComponent<ComboManager>();
        _playerComboManager.CurrComboMeterAmount = _playerComboManager.MaxComboMeterAmount;
    }
    public override bool ReceiveAttackRequest(IAttackRequester requester)
    {
        PlayerBattlePawn player = requester as PlayerBattlePawn;
        if (player != null)
        {
            _playerComboManager.CurrComboMeterAmount = _playerComboManager.MaxComboMeterAmount;
        }
        return true;
    }
}

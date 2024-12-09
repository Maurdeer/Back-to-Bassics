using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnAilment : StatusAilment
{
    private const int playerBurnDamage = 1;
    private const int enemyBurnDamage = 5;
    protected override void Start()
    {
        base.Start();
        _recoveryTime = 25;
    }
    protected override void OnFullBeat()
    {
        if (_pawn is PlayerBattlePawn)
        {
            _pawn.Damage(playerBurnDamage);
            UIManager.Instance.SetPlayerHealthBarOnFire();
        }
        else if (_pawn is EnemyBattlePawn)
        {
            _pawn.Damage(enemyBurnDamage);
        }
        
        base.OnFullBeat(); 
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UIManager.Instance.ExtinguishPlayerHealthBarOnFire();
    }
}

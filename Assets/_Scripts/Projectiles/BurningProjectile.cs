using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningProjectile : Projectile
{
    public override void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) Manual DEBUG UI Tracker -------
        BattleManager.Instance.Player.ApplyStatusAilment<BurnAilment>();
        base.OnAttackMaterialize(receiver);
    }
}

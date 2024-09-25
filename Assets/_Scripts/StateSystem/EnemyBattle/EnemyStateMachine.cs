using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EnemyStateMachine : StateMachine<EnemyStateMachine, EnemyStateMachine.EnemyState, EnemyStateInput>
{
    #region Unity Messages
    #endregion
    protected override void SetInitialState()
    {
        CurrInput.Enemy = GetComponent<EnemyBattlePawn>();
        CurrInput.EnemySprite = GetComponentInChildren<PawnSprite>();
        CurrInput.EnemyParticleSystem = GetComponentInChildren<ParticleSystem>();
        Transition<Idle>();   
    }
}

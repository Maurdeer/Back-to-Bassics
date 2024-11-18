using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyStateMachine;

public partial class EnemyStateMachine
{
    public class NotInBattle : EnemyState
    {
        public override void Enter(EnemyStateInput i)
        {
            base.Enter(i);
            if (Input.EnemySprite != null)
            {
                Input.EnemySprite.Animator?.Play("idle");
            }
        }
        public override bool AttackRequestHandler(IAttackRequester requester)
        {
            //Input.EnemySprite.Animator.Play("take_damage");
            Input.EnemyParticleSystem?.Play();
            return true;
        }
    }
}

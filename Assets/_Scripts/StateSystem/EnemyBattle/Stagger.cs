using UnityEngine;

public partial class EnemyStateMachine
{
    public class Stagger : EnemyState
    {
        public override void Enter(EnemyStateInput i)
        {
            base.Enter(i);
            Debug.Log("Staggered");
        }
        public override void Exit(EnemyStateInput i)
        {
            base.Exit(i);
            Debug.Log("UnStaggered");
        }
        public override bool AttackRequestHandler(IAttackRequester requester)
        {
            //Input.EnemySprite.Animator.Play("take_damage");
            // Move this somewhere else
            Input.EnemyParticleSystem.Play();
            return true;
        }
    }
}


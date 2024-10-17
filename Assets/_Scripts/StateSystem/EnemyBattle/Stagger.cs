using UnityEngine;

public partial class EnemyStateMachine
{
    public class Stagger : EnemyState
    {
        public override bool AttackRequestHandler(IAttackRequester requester)
        {
            Input.EnemySprite.Animator.Play("staggered_damaged");
            // Move this somewhere else
            Input.EnemyParticleSystem.Play();
            return true;
        }
    }
}


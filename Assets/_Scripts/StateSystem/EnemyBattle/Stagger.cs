using UnityEngine;

public partial class EnemyStateMachine
{
    public class Stagger : EnemyState
    {
        public override void Enter(EnemyStateInput i)
        {
            base.Enter(i);
            //Debug.Log("Staggered");
        }
        public override void Exit(EnemyStateInput i)
        {
            base.Exit(i);
            //Debug.Log("UnStaggered");
        }
        public override bool AttackRequestHandler(IAttackRequester requester)
        {
            return true;
        }
    }
}


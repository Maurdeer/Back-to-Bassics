using UnityEngine;

public partial class EnemyStateMachine
{
    public class Stagger : EnemyState
    {
        public override bool AttackRequestHandler(IAttackRequester requester)
        {
            return true;
        }
    }
}


public partial class EnemyStateMachine
{
    public class Dead : EnemyState
    {
        public override bool AttackRequestHandler(IAttackRequester requester)
        {
            return false;
        }

        public override void Enter(EnemyStateInput i)
        {
            base.Enter(i);
            if (Input.EnemySprite != null)
            {
                Input.EnemySprite?.Animator?.Play("standby");
                Input.EnemySprite.Animator?.Play("dead");
            }   
        }
    }
}


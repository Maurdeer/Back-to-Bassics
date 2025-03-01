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
                if (Input.Enemy.EnemyData.Name == "Bassics")
                {
                    Input.EnemySprite?.Animator?.Play("nothing");
                }
                else
                {
                    Input.EnemySprite?.Animator?.Play("StandbyFront");
                }
                
                Input.EnemySprite.Animator?.Play("dead");
            }   
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    public class Battle : GameState
    {
        public override void Enter(GameStateInput i)
        {
            GameManager.Instance.PC.SwitchToBattleActions();
            //CameraConfigure.Instance.SwitchToCamera(BattleManager.Instance.Enemy.battleCam);
            UIManager.Instance.ShowBattlePanel();
        }
        public override void Exit(GameStateInput i)
        {
            UIManager.Instance.HideBattlePanel();
        }
    }
}

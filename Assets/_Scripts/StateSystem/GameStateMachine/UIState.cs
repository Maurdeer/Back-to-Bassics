using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    public class UIState : GameState
    {
        public override void Enter(GameStateInput i)
        {
            GameManager.Instance.PC.DisableControl();
        }
        public override void Exit(GameStateInput i)
        {
            GameManager.Instance.PC.EnableControl();
        }
    }
}

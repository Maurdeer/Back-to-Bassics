using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    public class Death : GameState
    {
        public override void Enter(GameStateInput i)
        {
            GameManager.Instance.PC.SwitchToUIActions();
            UIManager.Instance.ShowDeathMenuPanel();
            DataPersistenceManager.instance.enabled = false;
        }
        public override void Exit(GameStateInput i)
        {
            UIManager.Instance.HideDeathMenuPanel();
            DataPersistenceManager.instance.enabled = true;
        }
    }
}

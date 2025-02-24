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
            if (DataPersistenceManager.Instance != null)
                DataPersistenceManager.Instance.enabled = false;
        }
        public override void Exit(GameStateInput i)
        {
            UIManager.Instance.HideDeathMenuPanel();
            if (DataPersistenceManager.Instance != null)
                DataPersistenceManager.Instance.enabled = true;
        }
    }
}

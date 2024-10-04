using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    public class Cutscene : GameState
    {
        public override void Enter(GameStateInput i)
        {
            GameManager.Instance.PC.SwitchToCutsceneActions();
            DataPersistenceManager.instance.DisableSaving();

        }
        public override void Exit(GameStateInput i)
        {
            DataPersistenceManager.instance.EnableSaving();
        }
    }
}

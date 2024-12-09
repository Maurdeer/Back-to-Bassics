using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    public class Pause : GameState
    {
        public override void Enter(GameStateInput i)
        {
            GameManager.Instance.PC.SwitchToUIActions();
            Conductor.Instance.PauseCondcutor();
            Time.timeScale = 0f;

        }
        public override void Exit(GameStateInput i)
        {
            // Make sure that you use the prev state property to go back correctly!
            Conductor.Instance.ResumeConductor();
            Time.timeScale = 1f;

        }
    }
}

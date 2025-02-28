using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    public class WorldTraversal : GameState
    {
        public override void Enter(GameStateInput i)
        {
            if (DataPersistenceManager.Instance != null)
                DataPersistenceManager.Instance.enabled = true;
            GameManager.Instance.PC.SwitchToTraversalActions();
            CameraConfigure.Instance.SwitchToCamera(Input.PonchoCam);
            UIManager.Instance.PauseButtonAnimator.Play("show");
        }

        public override void Exit(GameStateInput i)
        {
            UIManager.Instance.PauseButtonAnimator.Play("hide");
        }
    }
}

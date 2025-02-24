using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    public class WorldTraversal : GameState
    {
        public override void Enter(GameStateInput i)
        {
            DataPersistenceManager.instance.enabled = true;
            GameManager.Instance.PC.SwitchToTraversalActions();
            CameraConfigure.Instance.SwitchToCamera(Input.PonchoCam);
        }
    }
}

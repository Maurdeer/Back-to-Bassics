using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine : StateMachine<GameStateMachine, GameStateMachine.GameState, GameStateInput>
{
    protected override void SetInitialState()
    {
        CurrInput.PonchoCam = GameObject.Find("PonchoCam")?.GetComponent<CinemachineVirtualCamera>();
        BattleManager.Instance.Player = GameManager.Instance.PC.GetComponent<PlayerBattlePawn>();

        // Very Hacky
        if (IsOnState<Cutscene>()) return;

        Transition<WorldTraversal>();
    }
}

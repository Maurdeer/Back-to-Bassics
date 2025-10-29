using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwap : EnemyAction
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _virtualCamera;
    protected override void OnStartAction()
    {
        CameraConfigure.Instance.SwitchToCamera(_virtualCamera);
    }
}

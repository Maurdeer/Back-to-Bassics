using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalSpecific : MonoBehaviour
{
    public void GoToSecondCam(Cinemachine.CinemachineVirtualCamera camera)
    {
        CameraConfigure.Instance.SwitchToCamera(camera);
    }
}

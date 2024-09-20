using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraConfigure : Singleton<CameraConfigure>
{
    [SerializeField] private CinemachineVirtualCamera firstVirtualCamera;
    private CinemachineVirtualCamera curr;
    private CinemachineVirtualCamera prev;
    #region Unity Messages
    private void Awake()
    {
        InitializeSingleton(); 
    }
    private void Start()
    {
        curr = GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        // This shouldn't be needed please!!!
        if (curr == null)
        {
            Debug.LogError("Please Let Camera Config Reference a camera.");
            return;
        }
        prev = curr;
        curr.Priority = 10;
    }
    #endregion
    public void SwitchToCamera(CinemachineVirtualCamera targetCamera)
    {
        //curr.Priority = savedPriority;
        if (curr != null) curr.Priority = 1;
        prev = curr;
        curr = targetCamera;
        //savedPriority = curr.Priority;
        curr.Priority = 10;
        Debug.Log($"Curr: {curr}, Prev: {prev}");
    }
    public void SwitchBackToPrev()
    {
        SwitchToCamera(prev);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseControlStatusAilment : StatusAilment
{

    // Went the Visual Trick Route
    // or you can just flip the viewing camera to just let it be a visual trick instead of actually messing with the inputs.

    private PlayerController playerController;
    private Camera playerCamera;
    private bool controlsReversed = false;
    private Vector3 defaultCamPosition;

    public void reverse(float duration)
    {
        _recoveryTime = duration;
    }

    protected override void OnStartAction()
    {   
        base.OnStopAction();
       
        playerCamera = Camera.main; // I hope this is right ?

        if (playerCamera != null)
        {
            defaultCamPosition = playerCamera.transform.eulerAngles;

            playerCamera.transform.Rotate(0, 180f, 0);
        }
    }

    // Same Method as in StatusAliment
    protected override void OnFullBeat()
    {
        base.OnFullBeat();
        _currBuildUp -= _recoveryTime;

        if (_currBuildUp <= 0)
        {
            _currBuildUp = 0;
            Disable();
            Destroy(this);
        }
    }

    protected override void OnStopAction()
    {
        base.OnStopAction();

        
        if (playerCamera != null)
        {
            playerCamera.transform.eulerAngles = defaultCamPosition; 
        }
    }
}

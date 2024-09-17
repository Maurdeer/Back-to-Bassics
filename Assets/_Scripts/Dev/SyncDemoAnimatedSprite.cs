using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncDemoAnimatedSprite : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem playOnSuccess;
    
    [SerializeField]
    private ParticleSystem playOnAbort;

    private Vector3 initialRotation;

    [SerializeField] private float animationExtent = 5;

    private bool bopDirection = false;
    
    private void Awake()
    {
        initialRotation = transform.rotation.eulerAngles;
    }

    public void AnimateUpdate(float t)
    {
        transform.rotation = Quaternion.Euler(0, 0, animationExtent * t * (bopDirection ? -1 : 1)) * Quaternion.Euler(initialRotation);
    }

    public void AnimateStart()
    {
        transform.rotation = Quaternion.Euler(initialRotation);
    }

    public void AnimateComplete()
    {
        playOnSuccess.Play();
        transform.rotation = Quaternion.Euler(initialRotation);
        bopDirection = !bopDirection;
    }

    public void AnimateAbort()
    {
        playOnAbort.Play();
        transform.rotation = Quaternion.Euler(initialRotation);
        bopDirection = !bopDirection;
    }
}

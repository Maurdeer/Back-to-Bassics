using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]
public class StopAudioEventOnDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        GetComponent<StudioEventEmitter>().Stop();
    }
}

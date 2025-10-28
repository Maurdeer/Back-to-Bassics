using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : Singleton<FMODEvents>
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference beach { get; private set; }
    [field: SerializeField] public EventReference bassicsBlub { get; private set; }
    [field: SerializeField] public EventReference pointRinging { get; private set; }
    [field: SerializeField] public EventReference pointRingingEnd { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
[DisallowMultipleComponent]
public partial class UIManager : Singleton<UIManager>
{
    [field: Header("UI General")]
    [field: SerializeField] public PauseMenuCode pauseMenu { get; private set; }
    public void Awake()
    {
        InitializeSingleton();
    }
}

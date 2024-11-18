using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Burnable : MonoBehaviour
{
    private static HashSet<Burnable> instances = new HashSet<Burnable>();

    public static HashSet<Burnable> Instances { get => instances; set => instances = value; }

    private void Awake()
    {
        instances.Add(this);
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public abstract void Burn();

    public abstract void Extinguish();

}

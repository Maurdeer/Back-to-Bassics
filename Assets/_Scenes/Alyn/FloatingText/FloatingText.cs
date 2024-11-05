using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 3f;
    public Vector3 Offset;

    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}

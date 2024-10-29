using System;
using UnityEngine;

public class IndicatorLine : MonoBehaviour
{
    [NonSerialized] public bool isRight;

    void Update()
    {
        if (isRight)
        {
            gameObject.transform.localPosition += new Vector3(-2, 0, 0);
        }
        else
        {
            gameObject.transform.localPosition += new Vector3(2, 0, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localPosition += new Vector3(-2, 0, 0);
    }
}

using UnityEngine;

public class IndicatorLine : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.localPosition += new Vector3(-2, 0, 0);
    }
}

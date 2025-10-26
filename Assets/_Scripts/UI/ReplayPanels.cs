using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPanels : MonoBehaviour
{
    public void ShowPanelOnly(int idx)
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(i == idx);
            i++;
        }
    }
}

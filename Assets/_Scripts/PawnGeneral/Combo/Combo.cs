using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStateMachine;

public abstract class Combo : ScriptableObject
{
    [SerializeField] private string strId;
    [SerializeField] private int cost;
    public int Cost => cost;
    public string StrId => strId;
    public abstract void InBattle();
    public abstract void InTraversal();
    /// <summary>
    /// Starts combo attack if enough Combo Points
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool StartComboAttack(ComboManager manager)
    {
        if (GameManager.Instance.GSM.IsOnState<Battle>())
        {
            if (manager.CurrComboMeterAmount < cost) return false;
            InBattle();
            manager.CurrComboMeterAmount -= cost;
        }
        else if (GameManager.Instance.GSM.IsOnState<WorldTraversal>())
        {
            InTraversal();
        }
        return true;
    }
}

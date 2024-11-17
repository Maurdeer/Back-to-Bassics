using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [Header("Combo Starting")]
    [SerializeField] private Combo[] initialCombos;
    private Dictionary<string, Combo> combosDict;
    [Header("Combo Data")]
    [SerializeField] private int maxcomboMeterAmount = 100;
    [SerializeField] private int currComboMeterAmount;
    public int MaxComboMeterAmount => maxcomboMeterAmount;
    public int CurrComboMeterAmount
    {
        get { return currComboMeterAmount; }
        set
        {
            currComboMeterAmount = value;
            if (currComboMeterAmount > maxcomboMeterAmount)
            {
                currComboMeterAmount = maxcomboMeterAmount;
            }
            if (currComboMeterAmount < 0)
            {
                currComboMeterAmount = 0;
            }
            UIManager.Instance.UpdateComboMeter(this);
        }
    }

    private string _currComboString;
    private Coroutine _comboReseter;
    private void Awake()
    {
        _currComboString = "";
        combosDict = new Dictionary<string, Combo>();
        foreach (Combo combo in initialCombos)
        {
            combosDict.Add(combo.StrId, combo);
        }
    }
    public void AppendToCombo(char combo)
    {
        if (_currComboString.Length >= 4)
        {
            _currComboString = "";
        }
        
        _currComboString += combo;
        UIManager.Instance.ComboDisplay.AddCombo(combo);
        if (_comboReseter != null)
        {
            StopCoroutine(_comboReseter);
        }
        _comboReseter = StartCoroutine(DelayComboReset());
        if (!combosDict.ContainsKey(_currComboString)) return;
        if (!combosDict[_currComboString].StartComboAttack(this))
        {
            UIManager.Instance.ComboDisplay.HideCombo();
            _currComboString = "";
            StopCoroutine(_comboReseter);
            return;
        }
        Debug.Log("CurrCombo: " + _currComboString);
        UIManager.Instance.ComboDisplay.ValidCombo();
    }
    private IEnumerator DelayComboReset()
    {
        // Give the player 2 beat of time
        if (Conductor.Instance.IsPlaying)
        {
            yield return new WaitForSeconds(2 * Conductor.Instance.spb);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        
        UIManager.Instance.ComboDisplay.HideCombo();
        _currComboString = "";
    }
    public void AddCombo(Combo combo)
    {
        combosDict.Add(combo.StrId, combo);
    }
}

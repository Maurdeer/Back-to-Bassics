using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour that uses Conductor beating behaviour
/// </summary>
public abstract class Conductable : MonoBehaviour
{
    private bool _conductableEnabled;
    protected virtual void OnFirstBeat() { }
    protected virtual void OnQuarterBeat() { }
    protected virtual void OnHalfBeat() { }
    protected virtual void OnFullBeat() { }
    protected virtual void OnLastBeat() { }
    public void Enable()
    {
        if (_conductableEnabled) return;
        _conductableEnabled = true;
        Conductor.Instance.OnFirstBeat += OnFirstBeat;
        // Conductor.Instance.OnQuarterBeat += OnQuarterBeat;
        // Conductor.Instance.OnHalfBeat += OnHalfBeat;
        Conductor.Instance.OnFullBeat += OnFullBeat;
        // Conductor.Instance.OnLastBeat += OnLastBeat;
    }
    public void Disable()
    {
        if (!_conductableEnabled) return;
        _conductableEnabled = false;
        Conductor.Instance.OnFirstBeat -= OnFirstBeat;
        // Conductor.Instance.OnQuarterBeat -= OnQuarterBeat;
        // Conductor.Instance.OnHalfBeat -= OnHalfBeat;
        Conductor.Instance.OnFullBeat -= OnFullBeat;
        // Conductor.Instance.OnLastBeat -= OnLastBeat;
    }
}

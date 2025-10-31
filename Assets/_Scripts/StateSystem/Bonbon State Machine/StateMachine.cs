using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

//The following code is heavily influenced from a state machine used in Side By Side (Producer: Yoon Lee)
//Repurposed for Dive
//Stolen by Bonbon

/// <summary>
/// Defines a Finite State Machine that can be extended for more functionality.
/// </summary>
/// <typeparam name="S">An abstract class that defines what kind of state you want</typeparam>
/// <typeparam name="I">A class that carries values between states</typeparam>
public abstract class StateMachine<M, S, I> : MonoBehaviour
    where M : StateMachine<M, S, I>
    where S : State<M, S, I>
    where I : StateInput
{
    public S CurrState { get; private set; }
    public S PrevState { get; private set; }
    public I CurrInput { get; private set; }

    private Dictionary<Type, S> _stateMap = new Dictionary<Type, S>();
    
    private IEnumerator _transitionAction = null;
    private bool _locked = false;

    public event Action StateTransition; //(State, PrevState)

    #region Unity Messages

    protected virtual void Awake() => CurrInput = (I) Activator.CreateInstance(typeof(I));

    protected virtual void Start() {
        Init();

        //The below code was provided by Side By Side (Producer: Yoon Lee), who got it from Brandon Shockley
        //Gets all inherited classes of S and instantiates them using voodoo magic code I got from Brandon Shockley lol
        //EDIT by Logan Bowers: Removed Voodoo magic code that gets all the types because you can add a generic constraint that forces the type to be correct so no errors occur.
        //EDIT EDIT: Keeping the Voodoo magic code here in honor of our fallen brother, and because it works, as it should.
        foreach (Type type in Assembly.GetAssembly(typeof(S)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(S))))
        {
            S newState = (S)Activator.CreateInstance(type);
            newState.MySM = (M)this;
            // newState.character = this;
            // newState.Init(stateInput);
            if (!_stateMap.ContainsKey(type))
                _stateMap.Add(type, newState);
            // loadedStates.Add(type.FullName, RuntimeHelpers.GetHashCode(newState).ToString());
        }
        SetInitialState();
    }

    protected virtual void Update() {
        CurrState?.Update();
    }
    
    protected virtual void FixedUpdate() {
        CurrState?.FixedUpdate();
    }
    #endregion

    public void Transition<NextStateType>() where NextStateType : S, new() {
        if (_locked) return;

        //Debug.Log("Transition To State: " + typeof(NextStateType).Name);
        CurrState?.Exit(CurrInput);
        SetState<NextStateType>();
        CurrState.Enter(CurrInput);

        StateTransition?.Invoke();
    }
    
    //Edit by Chris Lee: Added delayed transition, intended for programmed UI animation between states :O
    public void DelayedTransition<NextStateType>(float delay, bool overrideCurrState) where NextStateType : S, new() {
        if (_locked) return;

        if (overrideCurrState) _transitionAction = null;
        if (_transitionAction == null) {
            _transitionAction = DelayedTransitionAction<NextStateType>(delay);
            StartCoroutine(_transitionAction);
        }
    }

    private IEnumerator DelayedTransitionAction<NextStateType>(float delay) where NextStateType : S, new() {
        CurrState?.Exit(CurrInput);
        yield return new WaitForSeconds(delay);
        SetState<NextStateType>();
        CurrState.Enter(CurrInput);
        
        StateTransition?.Invoke();
        _transitionAction = null;
        yield return null;
    }

    public bool IsOnState<CheckStateType>() where CheckStateType : S, new() {
        return CurrState.GetType() == typeof(CheckStateType);
    }

    protected void SetState<T>() where T : S, new()
    {
        if (_locked) return;

        Type stateType = typeof(T);
        if (!_stateMap.ContainsKey(stateType)) {
            CreateNewState(stateType);
        }

        PrevState = CurrState;
        CurrState = _stateMap[stateType];
        if (PrevState == null) PrevState = CurrState;
    }

    public bool PrevStateEquals<T>() where T : S
    {
        return typeof(T) == PrevState.GetType();
    }

    private void CreateNewState(Type state)
    {
        S newState = (S) Activator.CreateInstance(state);
        newState.MySM = (M) this;
        _stateMap.Add(state, newState);
    }

    //Edit by Chris Lee: Functionality for disabling state transitions, resumes next state if enabled
    public virtual void ToggleMachine(bool disable) {
        _locked = disable;
    }

    protected virtual void Init() { }

    //Ideally this would return the initial state, but idk how to do that with generics.
    protected abstract void SetInitialState();
}

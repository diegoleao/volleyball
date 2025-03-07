using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateMachine : MonoBehaviour
{
    private BaseState currentState;
    private BaseState previousState; 
    
    private Queue<BaseState> stateQueue = new Queue<BaseState>();

    private Dictionary<Type,BaseState> states = new Dictionary<Type, BaseState>();

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        FillDictionaryWith(CreateAllStates());

    }

    private void FillDictionaryWith(List<BaseState> statesList)
    {
        foreach (BaseState state in statesList)
        {
            states.Add(state.GetType(), state);
        }
    }

    public abstract List<BaseState> CreateAllStates();

    public void QueueNextState<T>() where T : BaseState
    {
        stateQueue.Enqueue(states[typeof(T)]);

    }

    private void SetState(BaseState newState)
    {
        if (currentState != null && !currentState.CanTransitionTo(newState))
        {
            Debug.LogError($"[StateMachine] Invalid transition: {currentState.GetType()} → {newState.GetType()}");
            return;
        }

        Debug.Log($"[StateMachine] Change State: {currentState.GetType()} → {newState.GetType()}");
        previousState = currentState;
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();

    }

    void Update()
    {
        if (stateQueue.Count > 0)
        {
            SetState(stateQueue.Dequeue());
        }
        else
        {
            currentState?.Update();
        }

    }

}
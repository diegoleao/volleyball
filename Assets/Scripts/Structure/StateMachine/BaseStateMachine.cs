using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateMachine : MonoBehaviour
{

    private BaseState currentState;

    private BaseState previousState; 
    
    private Queue<BaseState> stateQueue = new Queue<BaseState>();

    private Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>();

    private AppCanvas appCanvas;
    private GameState gameState;
    private Type[] _interruptions = new Type[] { };
    public Type[] InterruptionStates
    {
        get
        {
            if (_interruptions.Length == 0)
            {
                _interruptions = GetInterruptionTypes();
            }
            return _interruptions;
        }

    }

    void Awake()
    {
        appCanvas = Provider.AppCanvas;
        gameState = Provider.GameState;
        Initialize();

    }

    public void Initialize()
    {
        FillStatesDictionaryWith(CreateAllStates());

        foreach (var state in states.Values)
        {
            state.Inject(gameState, this, appCanvas);
        }

    }

    public abstract List<BaseState> CreateAllStates();

    public abstract Type[] GetInterruptionTypes();

    private void FillStatesDictionaryWith(List<BaseState> statesList)
    {
        foreach (BaseState state in statesList)
        {
            states.Add(state.GetType(), state);
        }
    }

    public void QueueNext<T>() where T : BaseState
    {
        states.TryGetValue(typeof(T), out var state);
        if(state == null)
        {
            Debug.LogError($"Failed to Queue the State '{typeof(T).Name}', because it was not created by CreateAllStates().");
        }
        else
        {
            stateQueue.Enqueue(state);
        }

    }

    private void SetState(BaseState newState)
    {
        if (currentState != null && !currentState.CanTransitionTo(newState))
        {
            Debug.LogError($"[StateMachine] Invalid transition:                                                                 {currentState.GetType()} → {newState.GetType()}");
            return;
        }

#if UNITY_EDITOR
        Debug.Log($"[StateMachine]                                                                            {currentState?.GetType()} → {newState.GetType()}");
#endif

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
            currentState?.StateUpdate();
        }

    }

}
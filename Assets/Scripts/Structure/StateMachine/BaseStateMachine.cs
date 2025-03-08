using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateMachine : MonoBehaviour
{

    private const string LogPrefix = "[StateMachine]";

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
            DebugLogError($"Failed to Queue the State '{typeof(T).Name}', because it was not created by CreateAllStates().");
        }
        else
        {
            DebugLog($"Queueing State '{typeof(T).Name}'...");
            stateQueue.Enqueue(state);
        }

    }

    private void SetState(BaseState newState)
    {
        if (currentState != null && !currentState.CanTransitionTo(newState))
        {
            DebugLogError($"Invalid transition:" +
                $"                                                                           " +
                $"                                                                           " +
                $"                                                                    XXXX   " +
                $"{GetReadable(currentState)} → {GetReadable(newState)}   XXXX");
            return;
        }

#if UNITY_EDITOR
        DebugLog($"                         " +
                $"                                                                           " +
                $"                                                                           " +
                $"                                                                           " +
                $"{GetReadable(currentState)} → {GetReadable(newState)}");
#endif

        previousState = currentState;
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();

    }

    private static string GetReadable(BaseState newState)
    {
        if (newState == null) return "";

        return newState.GetType().Name.Replace("State", "");

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

    protected void DebugLogError(string message)
    {
        Debug.Log($"{LogPrefix} {message}");
    }

    protected void DebugLog(string message)
    {
        Debug.Log($"{LogPrefix} {message}");

    }

}
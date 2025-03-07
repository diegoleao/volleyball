using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseState : MonoBehaviour
{

    protected AppCanvas AppCanvas { get; private set; }
    protected StateMachine StateMachine { get; private set; }

    private HashSet<Type> AllowedTransitions = new HashSet<Type>();

    public BaseState AllowTransitionInto(params Type[] states)
    {
        states.ForEach(type => AllowedTransitions.Add(type));
        return this;
    }

    public bool CanTransitionTo<T>() where T : BaseState
    {
        return AllowedTransitions.Contains(typeof(T));

    }

    public bool CanTransitionTo(BaseState state)
    {
        return AllowedTransitions.Contains(state.GetType());

    }

    public void Inject(AppCanvas appCanvas, StateMachine stateMachine)
    {
        this.AppCanvas = appCanvas;
        this.StateMachine = stateMachine;

    }

    public abstract void OnCreate();

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void Update();

    protected void DebugLog(string message)
    {
        Debug.Log($"[State] {message}");
    }

}


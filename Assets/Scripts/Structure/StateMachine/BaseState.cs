using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseState
{

    protected AppCanvas AppCanvas { get; private set; }
    protected BaseStateMachine StateMachine { get; private set; }
    protected GameState GameState { get; private set; }

    private HashSet<Type> AllowedTransitions = new HashSet<Type>();

    private Type[] AllowedInterruptions;

    private bool allowsInterruptions;

    public BaseState TransitionsInto(params Type[] states)
    {
        states.ForEach(type => AllowedTransitions.Add(type));
        return this;

    }

    public BaseState AllowInterruptions()
    {
        allowsInterruptions = true;
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

    public void Inject(GameState gameState, BaseStateMachine stateMachine, AppCanvas appCanvas)
    {
        this.GameState = gameState;
        this.StateMachine = stateMachine;
        this.AppCanvas = appCanvas;
        if (this.allowsInterruptions)
        {
            this.TransitionsInto(StateMachine.InterruptionStates);
        }

    }

    public abstract void OnCreate();

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void StateUpdate();

    protected void DebugLog(string message)
    {
        Debug.Log($"[State] {message}");
    }

    protected void ShowSetFinishedLogMessage()
    {
#if UNITY_EDITOR
        Debug.Log($"Set finished. Final Score [ {GetScoresText()} ].");
#else
        Debug.Log("Set finished.");
#endif
    }

    protected void ShowMatchOngoingLogMessage()
    {
#if UNITY_EDITOR
        Debug.Log($"Set still ongoing. Current Score [ {GetScoresText()} ].");
#else
        Debug.Log("Set still ongoing.");
#endif
    }

#if UNITY_EDITOR
    protected string GetScoresText()
    {
        return $"A: {this.GameState.LocalMatchInfo.Scores[0].score} vs " +
               $"B: {this.GameState.LocalMatchInfo.Scores[1].score}";
    }
#endif

}


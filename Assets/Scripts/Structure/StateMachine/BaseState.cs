using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseState : MonoBehaviour
{
    protected HashSet<Type> AllowedTransitions = new HashSet<Type>();

    public BaseState AllowTransitionInto<T>() where T : BaseState
    {
        AllowedTransitions.Add(typeof(T));
        return this;
    }

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

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void Update();

}


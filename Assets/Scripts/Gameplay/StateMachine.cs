
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : BaseStateMachine
{

    [SerializeField] List<BaseState> baseStates;

    public override List<BaseState> CreateAllStates()
    {
        return new List<BaseState>()
        {
            new MainMenuState().AllowTransitionInto(typeof(StartMatchState)),

            new StartMatchState().AllowTransitionInto(typeof(DuringRallyState), 
                                                      typeof(AwardingPointsState)),

            new RallyStartState().AllowTransitionInto(typeof(RallyStartState), 
                                                      typeof(WinState)),

            new AwardingPointsState().AllowTransitionInto(typeof(RallyStartState)),

            new WinState().AllowTransitionInto(typeof(MainMenuState))

        };

    }

}
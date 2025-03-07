
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VolleyballStateMachine : BaseStateMachine
{

    [SerializeField] List<BaseState> baseStates;

    public override List<BaseState> CreateAllStates()
    {
        return new List<BaseState>()
        {
            new MainMenuState().AllowTransitionInto<StartMatchState>(),
            new StartMatchState().AllowTransitionInto(typeof(DuringRallyState), 
                                                      typeof(AwardingPointsState)),
            new RallyStartState().AllowTransitionInto(typeof(RallyStartState), 
                                                      typeof(WinState)),
            new AwardingPointsState().AllowTransitionInto<RallyStartState>(),
            new WinState().AllowTransitionInto<MainMenuState>()
        };

    }

}
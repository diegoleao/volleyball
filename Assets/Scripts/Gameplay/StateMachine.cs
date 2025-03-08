
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : BaseStateMachine
{
    public override List<BaseState> CreateAllStates()
    {
        return new List<BaseState>()
        {
            new MainMenuState()
                .TransitionsInto(typeof(MatchStartState), typeof(WaitForOpponentState)),

            new MatchStartState()
                .TransitionsInto(typeof(SetStartState))
                .AllowInterruptions(),

                new SetStartState()
                    .TransitionsInto(typeof(RallyStartState))
                    .AllowInterruptions(),

                    new RallyStartState()
                        .TransitionsInto(typeof(RallyOngoingState))
                        .AllowInterruptions(),

                    new RallyOngoingState()
                        .TransitionsInto(typeof(AwardingPointsState), typeof(RallyEndState))
                        .AllowInterruptions(),

                    new AwardingPointsState()
                        .TransitionsInto(typeof(RallyEndState))
                        .AllowInterruptions(),

                    new RallyEndState()
                        .TransitionsInto(typeof(RallyStartState), typeof(SetEndState))
                        .AllowInterruptions(),

                new SetEndState()
                    .TransitionsInto(typeof(SetStartState), typeof(MatchEnd_WinState)).AllowInterruptions(),

            new MatchEnd_WinState()
                .TransitionsInto(typeof(ShutdownState)),

            new RestartMatchState()
                .TransitionsInto(typeof(MatchStartState)),

            new AbortMatchState()
                .TransitionsInto(typeof(MainMenuState)),

            new WaitForOpponentState()
                .TransitionsInto(typeof(MainMenuState), typeof(MatchStartState))
                .AllowInterruptions(),

            new ShutdownState()
                .TransitionsInto(typeof(MainMenuState))

        };

    }

    public override Type[] GetInterruptionTypes()
    {
        return new Type[]{ typeof(AbortMatchState),
                           typeof(AwardingPointsState),     
                           typeof(RestartMatchState),
                           typeof(MainMenuState) };

    }

}

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
                .TransitionsInto(typeof(MatchStartState)),

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
                        .TransitionsInto(typeof(RallyEndState))
                        .AllowInterruptions(),

                    new AwardingPointsState()
                        .TransitionsInto(typeof(RallyStartState), typeof(RallyEndState))
                        .AllowInterruptions(),

                    new RallyEndState()
                        .TransitionsInto(typeof(SetFinishState))
                        .AllowInterruptions(),

                new SetFinishState()
                    .TransitionsInto(typeof(SetStartState)).AllowInterruptions(),

            new MatchEndState()
                .TransitionsInto(typeof(WinState)),

            new WinState()
                .TransitionsInto(typeof(MainMenuState)),

            new RestartMatchState()
                .TransitionsInto(typeof(MatchStartState)),

            new AbortMatchState()
                .TransitionsInto(typeof(MainMenuState)),

            new WaitForOpponentState()
                .TransitionsInto(typeof(MainMenuState))
                .AllowInterruptions()

        };

    }

    public override Type[] GetInterruptionTypes()
    {
        return new Type[]{ typeof(AbortMatchState),
                           typeof(RestartMatchState),
                           typeof(MainMenuState) };

    }

}
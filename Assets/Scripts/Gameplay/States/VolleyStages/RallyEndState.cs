
using System;
using UniRx;
using UnityEngine;

public class RallyEndState : BaseState
{

    public override void OnEnter()
    {
        if (this.GameState.LocalMatchInfo.IsSetFinished)
        {
            ShowSetFinishedLogMessage();
            StateMachine.QueueNext<SetEndState>();
        }
        else
        {
            StateMachine.QueueNext<RallyStartState>();
            ShowMatchOngoingLogMessage();
        }

    }

    public override void OnCreate()
    {

    }

    public override void OnExit()
    {

    }

    public override void StateUpdate()
    {

    }

}

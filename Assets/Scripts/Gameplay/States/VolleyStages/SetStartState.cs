
using System;
using UniRx;

public class SetStartState : BaseState
{

    public override void OnEnter()
    {
        //Show "SET X", then.. "START!"
        Provider.StateMachine.QueueNext<RallyStartState>();
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

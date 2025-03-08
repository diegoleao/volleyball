
using System;
using UniRx;

public class RallyStartState : BaseState
{

    public override void OnEnter()
    {
        Observable.Timer(TimeSpan.FromSeconds(this.GameState.BallSpawnDelay)).Subscribe(_ =>
        {
            Provider.API.SpawnVolleyball(this.GameState.ServingTeam);
            StateMachine.QueueNext<RallyOngoingState>();
        });

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


using UnityEngine;

public class AwardingPointsState : BaseState
{

    public override void OnEnter()
    {
        if (GameState.ResetPlayerPositionOnScore)
        {
            Provider.API.ResetPlayerPositions();
        }

        // Lock players positions
        // Show any "score!" animation
        // await "MatchInfo.AddPointTo" data to be propagated and only then:

        StateMachine.QueueNext<RallyEndState>();

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

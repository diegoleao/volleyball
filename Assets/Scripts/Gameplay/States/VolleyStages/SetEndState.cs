
public class SetEndState : BaseState
{

    public override void OnEnter()
    {
        if (this.GameState.LocalMatchInfo.IsMatchFinished)
        {
            ShowSetFinishedLogMessage();
            StateMachine.QueueNext<MatchEnd_WinState>();
        }
        else
        {
            ShowMatchOngoingLogMessage();
        }
        //else
        //   Show timed congratulatory message
        //***4 seconds later
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

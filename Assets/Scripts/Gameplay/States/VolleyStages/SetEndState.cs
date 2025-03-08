
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
            //   Show set congratulatory message
            //***4 seconds later
            Provider.API.ResetSet();
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

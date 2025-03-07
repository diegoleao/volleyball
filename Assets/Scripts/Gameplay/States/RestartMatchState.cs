
public class RestartMatchState : BaseState
{

    public override void OnEnter()
    {
        AppCanvas.GetView<OptionsScreen>().Show();
        Provider.API.ResetMatch();
        AppCanvas.GetView<WinScreen>()?.Close();
        //Provider.StateMachine.QueueNext<RallyStartState>();

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

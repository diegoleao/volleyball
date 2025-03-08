
public class MatchStartState : BaseState
{

    public override void OnEnter()
    {
        AppCanvas.GetView<WinScreen>()?.Close();
        AppCanvas.GetView<OptionsScreen>().Show();
        Provider.StateMachine.QueueNext<SetStartState>();
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

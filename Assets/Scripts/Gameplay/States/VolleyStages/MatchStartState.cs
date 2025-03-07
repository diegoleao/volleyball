
public class MatchStartState : BaseState
{

    public override void OnEnter()
    {
        AppCanvas.GetView<OptionsScreen>().Show();
        DebugLog("Player 2 entered. Match Start! =========");
        Provider.StateMachine.QueueNext<RallyStartState>();
        AppCanvas.GetView<WinScreen>()?.Close();
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

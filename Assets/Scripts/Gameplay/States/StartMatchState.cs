
public class StartMatchState : BaseState
{

    public override void OnEnter()
    {
        AppCanvas.GetView<OptionsScreen>().Show();
        DebugLog("Player 2 entered. Match Start! =========");
        Provider.StateMachine.QueueNextState<RallyStartState>();
        AppCanvas.GetView<WinScreen>()?.Close();
    }

    public override void OnCreate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

}

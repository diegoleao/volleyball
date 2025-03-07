
public class MatchEndState : BaseState
{

    public override void OnEnter()
    {
        Provider.API.ShutdownMatch();
        AppCanvas.GetView<OptionsScreen>().Hide();
        StateMachine.QueueNext<MainMenuState>();
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

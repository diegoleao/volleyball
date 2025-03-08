
public class MatchEnd_WinState : BaseState
{

    public override void OnEnter()
    {
        AppCanvas.GetView<OptionsScreen>().Hide();
        //   Show Match congratulatory message
        //Set players to winning positions
        //***4 seconds later
        ShowWiningAnimations();//await it
        AppCanvas.GetOrCreate<WinScreen>().SetData(this.GameState.WinningScore).Show();
        Provider.API.UnloadScene();

    }

    private void ShowWiningAnimations()
    {
        //Player winning and losing animations
        //Focus camera on winning players field to show them animating

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

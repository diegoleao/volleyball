
public class MatchEnd_WinState : BaseState
{

    public override void OnEnter()
    {
        AppCanvas.GetOrCreate<WinScreen>().SetData(this.GameState.WinningScore).Show();
        AppCanvas.GetView<OptionsScreen>().Hide();
        //Set players to winning positions
        ShowWiningAnimations();
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

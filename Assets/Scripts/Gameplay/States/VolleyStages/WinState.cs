
public class WinState : BaseState
{

    public override void OnEnter()
    {
        SetCourtToWinState();
        Provider.API.UnloadScene();

    }

    private void SetCourtToWinState()
    {
        AppCanvas.GetOrCreate<WinScreen>().SetData(this.GameState.WinningScore).Show();
        AppCanvas.GetView<OptionsScreen>().Hide();
        //Set players to winning positions
        ShowWiningAnimations();

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


public class WaitForOpponentState : BaseState
{

    public override void OnEnter()
    {
        DebugLog("Waiting for Player 2...");
        AppCanvas.GetView<OptionsScreen>().Show();
        //Show screen communicating the wait for another player
        //Show button to cancel the Match

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

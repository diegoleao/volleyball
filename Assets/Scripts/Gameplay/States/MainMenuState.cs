
using UnityEngine;

public class MainMenuState : BaseState
{

    public override void OnEnter()
    {
        AppCanvas.GetView<OptionsScreen>().Hide();
        AppCanvas.GetOrCreate<MainMenuScreen>().Show();
        AppCanvas.GetView<WinScreen>()?.Close();
        AppCanvas.GetView<HudView>().ResetScore();

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

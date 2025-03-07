
using UnityEngine;

public class MainMenuState : BaseState
{

    public override void OnEnter()
    {
        Provider.AppCanvas.GetView<OptionsScreen>().Hide();
        Provider.AppCanvas.GetOrCreate<MainMenuScreen>().Show();
        Provider.AppCanvas.GetView<WinScreen>()?.Close();
        Provider.AppCanvas.GetView<HudView>().ResetScore();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

}

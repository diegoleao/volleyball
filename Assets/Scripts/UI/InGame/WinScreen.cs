
using System;
using TMPro;
using UnityEngine;

public class WinScreen : BaseView
{

    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    private PlayerScoreData winningScore;

    protected override void OnFirstShow()
    {

    }

    public WinScreen SetData(PlayerScoreData winningScore)
    {
        this.winningScore = winningScore;
        this.textMeshProUGUI.text = $"Team \"{(winningScore.playerId == 0 ? "A" : "B")}\" Wins!";
        return this;

    }

    public void ConfirmButtonPressed()
    {
        this.Close();
        Provider.StateMachine.QueueNext<ShutdownState>();

    }
}
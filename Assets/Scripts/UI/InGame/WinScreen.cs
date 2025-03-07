
using System;
using TMPro;
using UnityEngine;

public class WinScreen : BaseView
{

    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    private PlayerScoreData winningScore;

    protected override void OnFirstExibition()
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
        Provider.Instance.GameState.SetState(GameState.State.FinishMatch);

    }
}
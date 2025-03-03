
using System;
using TMPro;
using UnityEngine;

public class WinScreen : BaseView
{

    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    private PlayerScoreData winningScore;

    public void SetData(PlayerScoreData winningScore)
    {
        this.winningScore = winningScore;
        this.textMeshProUGUI.text = $"Team \"{(winningScore.playerId == 0 ? "A" : "B")}\" Wins!";

    }

    public void ConfirmButtonPressed()
    {
        this.Close();
        Provider.Instance.GameState.SetState(GameState.State.FinishMatch);

    }
}
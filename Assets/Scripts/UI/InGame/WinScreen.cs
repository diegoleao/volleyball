
using System;
using TMPro;
using UnityEngine;

public class WinScreen : BaseView
{

    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    private MatchInfo.ScoreData winningScore;

    public void SetData(MatchInfo.ScoreData winningScore)
    {
        this.winningScore = winningScore;
        this.textMeshProUGUI.text = $"Team \"{(winningScore.playerId == 0 ? "A" : "B")}\" Wins!";

    }

    public void ConfirmButtonPressed()
    {
        this.Close();
        Provider.Instance.GameState.SetState(GameState.State.MatchEnded);

    }
}

using UniRx;
using System;
using TMPro;
using UnityEngine;

public class HudView : BaseView
{

    public TextMeshProUGUI ScoreTeamA;

    public TextMeshProUGUI ScoreTeamB;

    private MatchInfo matchInfo;

    public void Initialize(MatchInfo matchInfo)
    {
        ScoreTeamA.text = "00";
        ScoreTeamB.text = "00";
        this.matchInfo = matchInfo;
        matchInfo.ScoreChangedEvent.AddListener(newScore =>
        {
            ScoreTeamA.text = newScore[0].score.ToString("D2");
            ScoreTeamB.text = newScore[1].score.ToString("D2");
        });

    }

    public void ResetScore()
    {
        ScoreTeamA.text = "00";
        ScoreTeamB.text = "00";

    }

}
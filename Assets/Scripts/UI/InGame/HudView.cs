
using UniRx;
using System;
using TMPro;
using UnityEngine;

public class HudView : BaseView
{

    public TextMeshProUGUI ScoreTeamA;

    public TextMeshProUGUI ScoreTeamB;

    public void Initialize(LocalMatchInfo localMatchInfo)
    {
        ScoreTeamA.text = "00";
        ScoreTeamB.text = "00";
        localMatchInfo.ScoreChangedEvent.AddListener(newScore =>
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
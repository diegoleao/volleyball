
using UniRx;
using System;
using TMPro;
using UnityEngine;

public class HudView : BaseView
{

    public TextMeshProUGUI ScoreTeamA;

    public TextMeshProUGUI ScoreTeamB;

    private MatchInfo matchInfo;

    private void Start()
    {
        ScoreTeamA.text = "00";
        ScoreTeamB.text = "00";

        IDisposable disposable = null;
        disposable = Observable.EveryUpdate().Subscribe(_ =>
        {
            matchInfo = FindAnyObjectByType<MatchInfo>();

            if (matchInfo == null)
                return;

            matchInfo.ScoreChanged.AddListener(newScore =>
            {
                ScoreTeamA.text = newScore[0].score.ToString("D2");
                ScoreTeamB.text = newScore[1].score.ToString("D2");
            });

            disposable.Dispose();

        });

    }

    public void Close()
    {
        Destroy(this.gameObject);

    }

}
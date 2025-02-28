
using UniRx;
using System;
using TMPro;
using UnityEngine;

public class HudView : MonoBehaviour
{

    public TextMeshProUGUI ScorePlayer0;

    public TextMeshProUGUI ScorePlayer1;

    private MatchInfo matchInfo;

    private void Start()
    {
        ScorePlayer0.text = "P1: 0";
        ScorePlayer1.text = "P2: 0";

        IDisposable disposable = null;
        disposable = Observable.EveryUpdate().Subscribe(_ =>
        {
            matchInfo = FindAnyObjectByType<MatchInfo>();

            if (matchInfo == null)
                return;

            matchInfo.ScoreChanged.AddListener(newScore =>
            {
                ScorePlayer0.text = $"P1: {newScore[0].score}";
                ScorePlayer1.text = $"P2: {newScore[1].score}";
            });
            disposable.Dispose();

        });

    }

    public void Close()
    {
        Destroy(this.gameObject);

    }

}
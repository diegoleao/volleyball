

using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo : MonoBehaviour
{

    [Networked][ShowInInspector]
    public List<ScoreData> Scores { get; set; }

    public void Initialize(int player1, int player2)
    {
        Scores = new List<ScoreData>();
        Scores.Add(new ScoreData { playerId = player1 });
        Scores.Add(new ScoreData { playerId = player2 });

    }

    public void AddPointTo(int playerId)
    {
        var playerScore = this.Scores.Find(t=>t.playerId == playerId);
        if (playerScore != null)
        {
            playerScore.score++;
        }

    }

    [Serializable]
    public class ScoreData
    {
        public int playerId;
        public int score;
    }


}
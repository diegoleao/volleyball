using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LocalMatchInfo : MonoBehaviour
{
    private int MaxSetScore = 3;

    private const int TeamA_ID = 0;
    private const int TeamB_ID = 1;

    public bool HasLocalGameStarted { get; set; }

    public int ScoringTeam { get; set; }

    [ShowInInspector]
    [Sirenix.OdinInspector.ReadOnly]
    public bool IsSetFinished
    {
        get
        {
            return this.Scores.Any(scoreInfo => scoreInfo.score >= MaxSetScore);
        }

    }

    [ShowInInspector]
    [Sirenix.OdinInspector.ReadOnly]
    public bool IsMatchFinished
    {
        get
        {
            return IsSetFinished && (setResults.Count == 3);
        }

    }

    [Sirenix.OdinInspector.ReadOnly]
    public List<PlayerScoreData> Scores = new List<PlayerScoreData>();

    public UnityEvent<List<PlayerScoreData>> ScoreChangedEvent;

    public UnityEvent<Team> TeamScoreEvent;

    public UnityEvent<PlayerScoreData> SetWonEvent;

    private List<SetResult> setResults = new List<SetResult>();

    private Team tempScoringTeam { get; set; }

    public void InitializeLocal()
    {
        InitScores(player1: 0, player2: 1);
        Provider.Register<LocalMatchInfo>(this);

    }

    private void InitScores(int player1, int player2)
    {
        Scores = GetScoresAsList(player1, 0, player2, 0);
        ResetScoringTeam();
        ScoreChangedEvent.Invoke(Scores);

    }

    public void ResetScoringTeam()
    {
        ScoringTeam = (int)Team.None;

    }

    private void SetWinningPlayer(List<PlayerScoreData> newScores)
    {
        var setResult = new SetResult(newScores, MaxSetScore);

        if (setResult.HasWinningPlayer)
        {
            Debug.Log($"WINNER! Player: {setResult.WinningPlayerId} (Team {(Team)setResult.WinningPlayerId})");
            setResults.Add(setResult);
            SetWonEvent?.Invoke(setResult.WinningPlayerScore);
        }

    }

    public class SetResult
    {
        public bool HasWinningPlayer
        {
            get
            {
                return WinningPlayerScore != null;
            }
        }

        public int WinningPlayerId { get; private set; }

        public int WinningScore { get; private set; }

        public PlayerScoreData WinningPlayerScore { get; private set; }

        public List<PlayerScoreData> Scores { get; private set; }

        public SetResult(List<PlayerScoreData> scores, int maxScore)
        {
            this.Scores = scores;

            WinningPlayerScore = this.Scores.Find(t => t.score >= maxScore);

            if (HasWinningPlayer)
            {
                WinningPlayerId = WinningPlayerScore.playerId;
                WinningScore = WinningPlayerScore.score;
            }

        }
    }

    public void AddLocalScore(Team team)
    {
        int playerId = GetPlayerId(team);

        if (IsSetFinished)
        {
            Debug.Log($"MATCH FINISHED - IGNORING Score for Player Id {playerId} (Team {team})");
            return;
        }

        ScoringTeam = (int)team;
        HandleTeamScoreUpdate();

        GetPlayerScore(playerId).score++;
        HandleScoreUpdates(this.Scores);

    }

    public int GetPlayerId(Team team)
    {
        return (team == Team.A) ? TeamA_ID : TeamB_ID;
    }

    public bool HandleTeamScoreUpdate()
    {
        tempScoringTeam = (Team)ScoringTeam;

        if (tempScoringTeam == Team.None)
        {
            return false;
        }
        
        Debug.Log($"[LocaMatchInfo] Scoring Event for Team {tempScoringTeam}");
        TeamScoreEvent?.Invoke(tempScoringTeam);
        return true;

    }

    public void HandleScoreUpdates(List<PlayerScoreData> newScores)
    {
        this.Scores = newScores;

        if (IsSetFinished)
        {
            SetWinningPlayer(this.Scores);
        }

        ScoreChangedEvent?.Invoke(this.Scores);

    }

    public List<PlayerScoreData> GetScoresAsList(int player1, int scorePlayer1, int player2, int scorePlayer2)
    {
        return new List<PlayerScoreData>
        {
            new PlayerScoreData
            {
                playerId = player1,
                score = scorePlayer1
            },
            new PlayerScoreData
            {
                playerId = player2,
                score = scorePlayer2
            }
        };

    }

    private PlayerScoreData GetPlayerScore(int playerId)
    {
        return this.Scores.Find(t => t.playerId == playerId);

    }

    public void LocalResetMatch()
    {
        ResetScore();

    }

    public void LocalResetSet()
    {
        ResetScore();
        Provider.StateMachine.QueueNext<SetStartState>();

    }

    private void ResetScore()
    {
        Scores.ForEach(t => { t.score = 0; });
        ScoreChangedEvent?.Invoke(this.Scores);
    }

}

[Serializable]
public class PlayerScoreData
{
    public int playerId;
    public int score;
}
using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LocalMatchInfo : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] int MaxSetScore = 1;
#else
    const int MaxSetScore = 7;
#endif

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
            //if 3 games already ended:
            //  SetState(GameStates.MatchEnded);
            return true;//TODO: Implement match concept
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
            Debug.Log($"MATCH FINISHED - IGNORING Score for Player Id {playerId} (Team {(Team)playerId})");
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

    public void ResetScore()
    {
        Scores.ForEach(t => { t.score = 0; });
        HandleScoreUpdates(Scores);

    }

}

[Serializable]
public class PlayerScoreData
{
    public int playerId;
    public int score;
}
using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MatchInfo : NetworkBehaviour
{

    private const int TeamA_ID = 0;
    private const int TeamB_ID = 1;

    [ShowInInspector][Sirenix.OdinInspector.ReadOnly]
    public bool IsMatchFinished
    {
        get
        {
            return this.Score.Any(scoreInfo => scoreInfo.score >= 7);
        }

    }

    [Sirenix.OdinInspector.ReadOnly]
    public List<ScoreData> Score = new List<ScoreData>();

    public UnityEvent<List<ScoreData>> ScoreChangedEvent;

    public UnityEvent<Team> TeamScoreEvent;

    public UnityEvent<ScoreData> PlayerWonEvent;

    //Networked
    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedScore => default;

    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedPlayers => default;

    [Networked]
    private int ScoringTeam { get; set; }

    //Private
    private ChangeDetector _changeDetector;

    private Team tempScoringTeam;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        InitScores(player1: 0, player2: 1);//TODO: change it to real ids
        Provider.Register<MatchInfo>(this);

    }

    public void Update() 
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(NetworkedScore):
                    HandleScoreUpdates();
                    break;

                case nameof(ScoringTeam):
                    HandleTeamScoreUpdate();
                    break;
            }
        }
    }

    private void HandleTeamScoreUpdate()
    {
        tempScoringTeam = (Team)ScoringTeam;

        if (tempScoringTeam != Team.None)
        {
            TeamScoreEvent?.Invoke(tempScoringTeam);
            if (HasStateAuthority)
            {
                ResetScoringTeam();
            }

        }

    }

    private void ResetScoringTeam()
    {
        ScoringTeam = (int)Team.None;

    }

    private void HandleScoreUpdates()
    {
        var newScores = GetScoresAsList();

        if (!IsMatchFinished)
        {
            CheckWinningPlayer(newScores);
        }

        this.Score = newScores;
        ScoreChangedEvent?.Invoke(this.Score);

    }

    private void CheckWinningPlayer(List<ScoreData> newScores)
    {
        var winningPlayer = newScores.Find(t => t.score >= 7);
        if (winningPlayer != null)
        {
            Debug.Log($"WINNER! Player: {winningPlayer.playerId}");
            PlayerWonEvent?.Invoke(winningPlayer);
        }

    }

    private void InitScores(int player1, int player2)
    {
        NetworkedPlayers.Set(0, player1);
        NetworkedPlayers.Set(1, player2);
        NetworkedScore.Set(0, 0);
        NetworkedScore.Set(1, 0);
        Score = GetScoresAsList();
        ResetScoringTeam();
        ScoreChangedEvent.Invoke(Score);

    }

    public void AddScore(Team team)
    {
        int playerId = GetPlayerId(team);

        if (HasStateAuthority) // Only update on the authoritative side
        {
            if (IsMatchFinished)
            {
                Debug.Log($"MATCH FINISHED - IGNORING Score for Player Id {playerId}");
                return;
            }

            ScoringTeam = (int)team;

            var playerIndex = FindPlayerIndex(playerId);
            if (playerIndex >= 0)
            {
                NetworkedScore.Set(playerIndex, NetworkedScore[playerIndex] + 1);
            }
            else
            {
                Debug.LogError($"Can't ADD score to undefined player \"{playerId}\".");
            }
        }

    }

    public int GetScore(int playerId)
    {
        int playerIndex = FindPlayerIndex(playerId);
        if (playerIndex < 0)
        {
            Debug.LogError($"Can't GET score to undefined player \"{playerId}\".");
            return -1;
        }
        return NetworkedScore[playerIndex];

    }

    public void ResetScore()
    {
        NetworkedScore.Set(0, 0);
        NetworkedScore.Set(1, 0);
    }

    private int FindPlayerIndex(int playerId)
    {
        return this.NetworkedPlayers.ToList().FindIndex(t => t == playerId);

    }

    private List<ScoreData> GetScoresAsList()
    {
        return new List<ScoreData>
        {
            new ScoreData
            {
                playerId = NetworkedPlayers[0],
                score = NetworkedScore[0]
            },
            new ScoreData
            {
                playerId = NetworkedPlayers[1],
                score = NetworkedScore[1]
            }
        };

    }

    private static int GetPlayerId(Team team)
    {
        return (team == Team.A) ? TeamA_ID : TeamB_ID;
    }

    [Serializable]
    public class ScoreData
    {
        public int playerId;
        public int score;
    }


}
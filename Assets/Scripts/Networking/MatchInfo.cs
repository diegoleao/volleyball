using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MatchInfo : NetworkBehaviour
{
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

    public UnityEvent<ScoreData> PlayerWonEvent;

    //Networked
    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedScore => default;

    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedPlayers => default;

    //Private
    private ChangeDetector _changeDetector;

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
            }
        }
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
        ScoreChangedEvent.Invoke(Score);

    }

    public void AddScore(int playerId)
    {
        if (HasStateAuthority) // Only update on the authoritative side
        {
            if (IsMatchFinished)
            {
                Debug.Log($"MATCH FINISHED - IGNORING Score for Player Id {playerId}");
                return;
            }

            var playerIndex = FindPlayerIndex(playerId);
            if (playerIndex >= 0)
            {
                NetworkedScore.Set(playerIndex, NetworkedScore[playerIndex]+1);
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

    [Serializable]
    public class ScoreData
    {
        public int playerId;
        public int score;
    }


}
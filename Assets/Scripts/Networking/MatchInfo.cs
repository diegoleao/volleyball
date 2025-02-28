using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MatchInfo : NetworkBehaviour
{

    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedScore => default;

    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedPlayers => default;

    public List<ScoreData> ScoreList = new List<ScoreData>();

    public UnityEvent<List<ScoreData>> ScoreChanged;

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        Initialize(player1: 0, player2: 1);//TODO: change it to real ids

    }

    public void Update() 
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(NetworkedScore):
                    ScoreChanged?.Invoke(GetScoresAsList());
                    break;
            }
        }
    }

    public void Initialize(int player1, int player2)
    {
        NetworkedPlayers.Set(0, player1);
        NetworkedPlayers.Set(1, player2);
        NetworkedScore.Set(0, 0);
        NetworkedScore.Set(1, 0);
        ScoreList = GetScoresAsList();
        ScoreChanged.Invoke(ScoreList);

    }

    public void AddScore(int playerId)
    {
        if (HasStateAuthority) // Only update on the authoritative side
        {
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

    public void RestartMatch()
    {
        NetworkedScore.Set(0, 0);
        NetworkedScore.Set(1, 0);
    }

    [Serializable]
    public class ScoreData
    {
        public int playerId;
        public int score;
    }


}
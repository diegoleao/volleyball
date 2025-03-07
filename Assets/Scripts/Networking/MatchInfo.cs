using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LocalMatchInfo))]
public class MatchInfo : NetworkBehaviour
{
    private LocalMatchInfo localMatchInfo;
    public LocalMatchInfo LocalInfo => localMatchInfo;

    [Networked] public bool HasGameStarted { get; set; }

    public bool HasMatchStarted 
    { 
        get
        {
            return (Provider.NetworkMode == NetworkMode.Network) ? HasGameStarted : localMatchInfo.HasLocalGameStarted;
        }
        set
        {
            HasGameStarted = value;
            localMatchInfo.HasLocalGameStarted = value;

        }

    }

    [Networked]
    private int ScoringTeam { get; set; }

    public int CurrentScoringTeam
    {
        get
        {
            return (Provider.NetworkMode == NetworkMode.Network) ? ScoringTeam : localMatchInfo.ScoringTeam;
        }
        set
        {
            ScoringTeam = value;
            localMatchInfo.ScoringTeam = value;

        }

    }

    public bool IsMatchFinished
    {
        get
        {
            return localMatchInfo.IsMatchFinished;
        }
    }

    //Networked
    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedScore => default;

    [ShowInInspector]
    [Networked, Capacity(2)]
    private NetworkArray<int> NetworkedPlayers => default;

    //Private network variable
    private ChangeDetector changeDetector;

    public override void Spawned()
    {
        localMatchInfo = GetComponent<LocalMatchInfo>();
        HasMatchStarted = false;
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        InitNetworkScores(player1: 0, player2: 1);//TODO: change it to real ids
        Provider.Register<MatchInfo>(this);
    }

    private void InitNetworkScores(int player1, int player2)
    {
        NetworkedPlayers.Set(0, player1);
        NetworkedPlayers.Set(1, player2);
        NetworkedScore.Set(0, 0);
        NetworkedScore.Set(1, 0);
        localMatchInfo.Scores = GetNetworkedScoresAsList();
        ResetScoringTeam();
        localMatchInfo.ScoreChangedEvent.Invoke(localMatchInfo.Scores);

    }

    private void ResetScoringTeam()
    {
        CurrentScoringTeam = (int)Team.None;

    }

    public void Update() 
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(NetworkedScore):
                    Debug.Log($"[CHANGE DETECTOR] {change} VALUE: {NetworkedScore} =============");
                    this.localMatchInfo.HandleScoreUpdates(GetNetworkedScoresAsList());
                    break;

                case nameof(ScoringTeam):
                    Debug.Log($"[CHANGE DETECTOR] {change} VALUE: {(Team)ScoringTeam} =============");
                    if (this.localMatchInfo.HandleTeamScoreUpdate() && HasStateAuthority)
                    {
                        ResetScoringTeam();//Reset scoring team after each successful match point
                    }
                    break;

                case nameof(HasGameStarted):
                    Debug.Log($"[CHANGE DETECTOR] {change} VALUE: {HasGameStarted} =============");
                    Provider.GameState.StartGameplay();
                    break;

                case nameof(NetworkedPlayers):
                    //TODO: Make it possible for players to 
                    //choose Team A or Team B instead of automatically assigning a team
                    break;

                default:
                    Debug.LogError($"[CHANGE DETECTOR] UNKNOWN CHANGE TO: {change}");
                    break;
            }

        }

    }

    public void AddNetworkedScore(Team team)
    {
        int playerId = this.localMatchInfo.GetPlayerId(team);

        if (HasStateAuthority) // Only update on the authoritative side
        {
            if (localMatchInfo.IsMatchFinished)
            {
                Debug.Log($"[MatchInfo] MATCH FINISHED - IGNORING Score for Player Id {playerId}");
                return;
            }

            CurrentScoringTeam = (int)team;

            var playerIndex = FindNetworkedPlayerIndex(playerId);
            if (playerIndex >= 0)
            {
                Debug.Log($"[MatchInfo] ADDING NETWORKED SCORE to player \"{playerId}\".");
                NetworkedScore.Set(playerIndex, NetworkedScore[playerIndex] + 1);
            }
            else
            {
                Debug.LogError($"[MatchInfo] Can't ADD score to undefined player \"{playerId}\".");
            }

        }
    }

    private int GetNetworkedScore(int playerId)
    {
        int playerIndex = this.FindNetworkedPlayerIndex(playerId);
        if (playerIndex < 0)
        {
            Debug.LogError($"[MatchInfo] Can't GET score to undefined player \"{playerId}\".");
            return -1;
        }
        return NetworkedScore[playerIndex];

    }

    private int FindNetworkedPlayerIndex(int playerId)
    {
        return this.NetworkedPlayers.ToList().FindIndex(t => t == playerId);

    }

    [Button]
    public void RequestMatchReset()
    {
        if (Runner.IsClient)
        {
            RPC_RequestRestart();
        }
        else
        {
            MatchReset();
        }

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestRestart(RpcInfo info = default)
    {
        if (Runner.IsServer)
        {
            MatchReset();
        }
    }

    private void MatchReset()
    {
        Provider.GameplayFacade.GameNetworking.ResetPlayerPositions();
        Provider.GameplayFacade.GameNetworking.DestroyAllBalls();
        ResetScores();

    }

    private void ResetScores()
    {
        NetworkedScore.Set(0, 0);
        NetworkedScore.Set(1, 0);

    }

    private List<PlayerScoreData> GetNetworkedScoresAsList()
    {
        return localMatchInfo.GetScoresAsList(NetworkedPlayers[0], NetworkedScore[0], NetworkedPlayers[1], NetworkedScore[1]);

    }

}
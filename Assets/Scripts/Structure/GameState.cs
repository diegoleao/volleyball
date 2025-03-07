using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public float BallSpawnDelay = 1f;

    public bool ResetPlayerPositionOnScore;

    public Team ServingTeam { get; private set; }

    public PlayerScoreData WinningScore { get; internal set; }

    private MatchInfo matchInfo;

    private LocalMatchInfo localMatchInfo;

    private AppCanvas AppCanvas;

    private StateMachine StateMachine;

    public LocalMatchInfo LocalMatchInfo
    {
        get
        {
            if (Provider.NetworkMode == NetworkMode.Network)
            {
                return matchInfo.LocalInfo;
            }

            return localMatchInfo;

        }

    }

    public void Initialize()
    {
        AppCanvas = Provider.AppCanvas;
        AppCanvas.GetOrCreatePopup<OptionsScreen>();

        StateMachine = Provider.StateMachine;
        StateMachine.QueueNext<MainMenuState>();

    }

    public void StartMultiplayerMatch(string roomName, GameMode mode)
    {
        Provider.GameplayFacade.StartNetworkMatch(roomName, mode, () =>
        {
            StateMachine.QueueNext<WaitForOpponentState>();
        },
        () =>
        {
            Debug.Log($"Unable to enter as {mode}");
            //Handle error
        });
        
    }

    public void StartSingleplayerMatch()
    {
        StateMachine.QueueNext<MatchStartState>();

    }

    //Used by UI
    public void RestartMatch()
    {
        StateMachine.QueueNext<RestartMatchState>();

    }

    public void HandleTeamScoring(Team scoringTeam)
    {
        if (this.LocalMatchInfo.IsMatchFinished)
            return;

        if (scoringTeam == Team.None)
            return;

        this.ServingTeam = scoringTeam;

    }

    public void HandlePlayerWinning(PlayerScoreData scoreData)
    {
        this.WinningScore = scoreData;
        StateMachine.QueueNext<WinState>();

    }

    public void IncreaseNetworkedScoreFor(Team team)
    {
        if (Provider.HasStateAuthority)
        {
            Debug.Log($"[GameState][Networked] Increase Score for Team {team}");
            this.matchInfo.AddNetworkedScore(team);
        }

    }

    public void IncreaseLocalScoreFor(Team team)
    {
        Debug.Log($"[GameState] Increase Score for Team {team}");
        this.localMatchInfo.AddLocalScore(team);

    }

    private void ResetCourtState()
    {
        Provider.API.ResetPlayerPositions();

    }

    public void ExitMatch()
    {
        ReturnToMainScreen();

    }

    private void ReturnToMainScreen()
    {
        StateMachine.QueueNext<MainMenuState>();
    }


    //Used by UI
    public void AbortMatch()
    {
        StateMachine.QueueNext<AbortMatchState>();
        
    }

    public void SetMatchInfo(MatchInfo matchInfo)
    {
        this.matchInfo = matchInfo;
        SetLocalMatchInfo(matchInfo.LocalInfo);

    }

    public void SetLocalMatchInfo(LocalMatchInfo localMatchInfo)
    {
        this.localMatchInfo = localMatchInfo;
        localMatchInfo.TeamScoreEvent.AddListener(HandleTeamScoring);
        localMatchInfo.ScoreChangedEvent.AddListener(HandleScoreChanged);
        localMatchInfo.PlayerWonEvent.AddListener(HandlePlayerWinning);

    }

    private void HandleScoreChanged(List<PlayerScoreData> scores)
    {
        if (LocalMatchInfo.IsMatchFinished)
            return;

        if (scores.All(t => t.score == 0))
        {
            Provider.StateMachine.QueueNext<RallyStartState>();
        }
        else
        {
            StateMachine.QueueNext<AwardingPointsState>();
        }

    }

    public void StartGameplay()
    {
        StateMachine.QueueNext<MatchStartState>();

    }

    public void ToggleDebug()
    {
        Provider.CourtTriggers.ToggleDebugVolumes();

    }

}

[Serializable]
public enum Team
{
    None,
    A,
    B
}

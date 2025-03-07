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
    [SerializeField] float BallSpawnDelay = 1f;

    [SerializeField] bool ResetPlayerPositionOnScore;

    public Team ServingTeam { get; private set; }

    public PlayerScoreData winningScore { get; internal set; }


    private WinScreen winScreenInstance;

    private State state;

    private State previousState;

    private MatchInfo matchInfo;

    private LocalMatchInfo localMatchInfo;

    private AppCanvas AppCanvas;

    public void Initialize()
    {
        AppCanvas = Provider.AppCanvas;
        AppCanvas.GetOrCreatePopup<OptionsScreen>();
        SetState(State.Menu);

    }

    public void StartMultiplayerMatch(string roomName, GameMode mode)
    {
        Provider.Instance.GameplayFacade.StartNetworkMatch(roomName, mode, () =>
        {
            SetState(State.WaitForPlayer2);
        },
        () =>
        {
            Debug.Log($"Unable to enter as {mode}");
            //Handle error
        });
        
    }

    public void StartSingleplayerMatch()
    {
        SetState(State.StartMatch);

    }

    //Used by UI
    public void RestartMatch()
    {
        SetState(State.RestartMatch);

    }

    public void SetState(State newState)
    {
        this.previousState = this.state;
        this.state = newState;

        switch (this.state)
        {
            case State.Menu:
                AppCanvas.GetView<OptionsScreen>().Hide();
                AppCanvas.GetOrCreate<MainMenuScreen>().Show();
                AppCanvas.GetView<WinScreen>()?.Close();
                AppCanvas.GetView<HudView>().ResetScore();
                break;

            case State.WaitForPlayer2:
                Debug.Log("Waiting for Player 2...");
                AppCanvas.GetView<OptionsScreen>().Show();
                //Show screen communicating the wait for another player
                //Show button to cancel the Match
                break;

            case State.StartMatch:
                AppCanvas.GetView<OptionsScreen>().Show();
                Debug.Log("Player 2 entered. Match Start! =========");
                SetState(State.RallyStart);
                winScreenInstance?.Close();
                break;

            case State.RestartMatch:
                AppCanvas.GetView<OptionsScreen>().Show();
                Provider.Instance.API.ResetMatch();
                winScreenInstance?.Close();
                DelaySetRallyStartState();
                break;

            case State.RallyStart:
                if(this.previousState != State.RallyStart)
                {
                    Observable.Timer(TimeSpan.FromSeconds(BallSpawnDelay)).Subscribe(_ =>
                    {
                        Provider.Instance.API.SpawnVolleyball(ServingTeam);
                    });
                }
                break;

            case State.AwardingPoints:
                if (ResetPlayerPositionOnScore)
                {
                    Provider.Instance.API.ResetPlayerPositions();
                }   
                // Lock players positions
                // Show any "score!" animation
                // await "MatchInfo.AddPointTo" data to be propagated and only then:
                if (!this.GetLocalMatchInfo().IsMatchFinished) 
                { 
                    SetState(State.RallyStart);
#if UNITY_EDITOR
                    Debug.Log($"Match still ongoing at A: {GetLocalMatchInfo().Scores[0].score} vs B: {GetLocalMatchInfo().Scores[1].score}.");
#endif
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log($"Match finished, skipping Rally Start. Final Score A: {GetLocalMatchInfo().Scores[0].score} vs B: {GetLocalMatchInfo().Scores[1].score}.");
#else
                    Debug.Log("Match finished, Skipping Rally Start.");
#endif
                }
                break;

            case State.SetFinished:
                //if 3 games already ended:
                //  SetState(GameStates.MatchEnded);
                //else
                //   Show timed congratulatory message
                //***4 seconds later
                break;

            case State.WinState:
                //Show end screen with results
                AppCanvas.GetOrCreate<WinScreen>().SetData(winningScore).Show();
                //***Wait for a few seconds
                SetCourtToWinState();
                AppCanvas.GetView<OptionsScreen>().Hide();
                Provider.Instance.API.UnloadScene();
                break;

            case State.FinishMatch:
            case State.AbortMatch:
                Provider.Instance.API.ShutdownNetworkMatch();
                ReturnToMainScreen();
                AppCanvas.GetView<OptionsScreen>().Hide();
                break;

            default:
                break;

        }

    }

    private void DelaySetRallyStartState()
    {
        Observable.NextFrame().Subscribe(_ => { SetState(State.RallyStart); });
    }

    public void ResetMatch()
    {
        

    }

    private void ShowWiningAnimations()
    {
        //Player winning and losing animations
        //Focus camera on winning players field to show them animating
    }

    public void HandleTeamScoring(Team scoringTeam)
    {
        if (GetLocalMatchInfo().IsMatchFinished)
            return;

        if (scoringTeam == Team.None)
            return;

        this.ServingTeam = scoringTeam;

    }

    public void HandlePlayerWinning(PlayerScoreData scoreData)
    {
        this.winningScore = scoreData;
        Provider.Instance.GameState.SetState(GameState.State.WinState);

    }

    public void IncreaseNetworkedScoreFor(Team team)
    {
        if (Provider.Instance.HasStateAuthority)
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
        Provider.Instance.API.ResetPlayerPositions();

    }

    private void SetCourtToWinState()
    {
        //Provider.Instance.GameNetworking.ResetPlayerPositions();
        ShowWiningAnimations();

    }

    public void ExitMatch()
    {
        ReturnToMainScreen();

    }

    private void ReturnToMainScreen()
    {
        SetState(State.Menu);
    }


    //Used by UI
    public void AbortMatch()
    {
        SetState(State.AbortMatch);
        
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

    private LocalMatchInfo GetLocalMatchInfo()
    {
        if(Provider.Instance.NetworkMode == NetworkMode.Network)
        {
            return matchInfo.LocalInfo;
        }
        else
        {
            return localMatchInfo;
        }
        
    }

    private void HandleScoreChanged(List<PlayerScoreData> scores)
    {
        if (GetLocalMatchInfo().IsMatchFinished)
            return;

        if (scores.All(t => t.score == 0))
        {
            DelaySetRallyStartState();
        }

        SetState(State.AwardingPoints);

    }

    public void StartGameplay()
    {
        SetState(State.StartMatch);

    }

    public void ToggleDebug()
    {
        Provider.Instance.CourtTriggers.ToggleDebugVolumes();

    }

    [Serializable]
    public enum State
    {
        Menu,
        StartMatch,
        RestartMatch,
        SetStart,
        RallyStart,
        DuringRally,
        AwardingPoints,
        SetFinished,
        WinState,
        FinishMatch,
        AbortMatch,
        WaitForPlayer2
    }

}

[Serializable]
public enum Team
{
    None,
    A,
    B
}

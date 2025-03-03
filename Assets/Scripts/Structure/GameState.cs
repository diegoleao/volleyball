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

    [SerializeField] bool ResetPlayerPositionOnScore;

    [SerializeField] MainMenuScreen MainMenu;

    [SerializeField] WinScreen WinScreen;

    public Team ServingTeam { get; private set; }

    public PlayerScoreData winningScore { get; internal set; }


    private WinScreen winScreenInstance;

    private State state;

    private MatchInfo matchInfo;


    public void Initialize()
    {
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

    public void SetState(State state)
    {
        switch (state)
        {
            case State.Menu:
                FindAnyObjectByType<OptionsScreen>().Hide();
                Instantiate(MainMenu);
                winScreenInstance?.Close();
                FindAnyObjectByType<HudView>().ResetScore();
                break;

            case State.WaitForPlayer2:
                Debug.Log("Waiting for Player 2...");
                FindAnyObjectByType<OptionsScreen>().Show();
                //Show screen communicating the wait for another player
                //Show button to cancel the Match
                break;

            case State.StartMatch:
                Debug.Log("Player 2 entered. Match Start! =========");
                SetState(State.RallyStart);
                winScreenInstance?.Close();
                break;

            case State.RestartMatch:
                FindAnyObjectByType<OptionsScreen>().Show();
                ResetMatch();
                SetState(State.RallyStart);
                winScreenInstance?.Close();
                break;

            case State.RallyStart:
                if(this.state != State.RallyStart)
                {
                    Provider.Instance.BallSpawner.SpawnVolleyball(ServingTeam);
                }
                break;

            case State.AwardingPoints:
                if(ResetPlayerPositionOnScore)
                    Provider.Instance.API.ResetPlayerPositions();
                // Lock players positions
                // Show any "score!" animation
                // await "MatchInfo.AddPointTo" data to be propagated and only then:
                if (!this.matchInfo.IsMatchFinished) 
                { 
                    SetState(State.RallyStart); 
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
                winScreenInstance = Instantiate(WinScreen);
                winScreenInstance.SetData(winningScore);
                //***Wait for a few seconds
                SetCourtToWinState();
                FindAnyObjectByType<OptionsScreen>().Hide();
                break;

            case State.FinishMatch:
            case State.AbortMatch:
                Provider.Instance.API.ShutdownNetworkMatch();
                ReturnToMainScreen();
                FindAnyObjectByType<OptionsScreen>().Hide();
                break;

            default:
                break;

        }
        this.state = state;

    }

    public void ResetMatch()
    {
        Provider.Instance.API.ResetMatch();

    }

    private void ShowWiningAnimations()
    {
        //Player winning and losing animations
        //Focus camera on winning players field to show them animating
    }

    public void HandleTeamScoring(Team scoringTeam)
    {
        if (matchInfo.IsMatchFinished)
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

    [Button]
    public void IncreaseScoreFor(Team team)
    {
        if (Provider.Instance.HasStateAuthority)
        {
            Debug.Log($"Increase Score for Team {team}");
            this.matchInfo.AddNetworkedScore(team);

        }

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

        matchInfo.LocalInfo.TeamScoreEvent.AddListener(HandleTeamScoring);
        matchInfo.LocalInfo.ScoreChangedEvent.AddListener(HandleScoreChanged);
        matchInfo.LocalInfo.PlayerWonEvent.AddListener(HandlePlayerWinning);

    }

    private void HandleScoreChanged(List<PlayerScoreData> scores)
    {
        if (matchInfo.IsMatchFinished)
            return;

        if (scores.All(t => t.score == 0))
            return;

        SetState(State.AwardingPoints);

    }

    public void StartGameplay()
    {
        SetState(GameState.State.StartMatch);

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
    A,
    B,
    None
}

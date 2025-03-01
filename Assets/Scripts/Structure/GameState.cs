using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GameState : MonoBehaviour
{

    [SerializeField] MainMenuScreen MainMenu;

    [SerializeField] WinScreen WinScreen;

    public MatchInfo.ScoreData winningScore { get; internal set; }


    private WinScreen winScreenInstance;

    private State state;

    private MatchInfo matchInfo;


    public void Initialize()
    {
        SetState(State.Menu);

    }

    public void StartMultiplayerMatch(string roomName, GameMode mode)
    {
        Provider.Instance.GameNetworking.StartNetwork(roomName, mode, () =>
        {
            SetState(State.StartMatch);
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
        this.state = state;
        switch (state)
        {
            case State.Menu:
                FindAnyObjectByType<OptionsScreen>().Hide();
                Instantiate(MainMenu);
                winScreenInstance?.Close();
                FindAnyObjectByType<HudView>().ResetScore();
                break;

            case State.StartMatch:
                FindAnyObjectByType<OptionsScreen>().Show();
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
                //Provider.Instance.BallSpawner.Spawn();
                break;

            case State.AwardingPoints:
                // Lock players positions
                // Show any "score!" animation
                // await "MatchInfo.AddPointTo" data to be propagated and only then:
                break;

            case State.SetFinished:
                //if 3 games already ended:
                //  SetState(GameStates.MatchEnded);
                //else
                //   Show timed congratulatory message
                //***4 seconds later
                break;

            case State.WinResultsCheck:
                //Show end screen with results
                winScreenInstance = Instantiate(WinScreen);
                winScreenInstance.SetData(winningScore);
                //***Wait for a few seconds
                SetCourtToWinState();
                FindAnyObjectByType<OptionsScreen>().Hide();
                break;

            case State.MatchEnded:
            case State.AbortMatch:
                ReturnToMainScreen();
                Provider.Instance.GameNetworking.ShutdownNetworkMatch();
                FindAnyObjectByType<OptionsScreen>().Hide();
                break;

        }

    }

    public void ResetMatch()
    {
        Provider.Instance.GameNetworking.ResetMatch();

    }

    private void ShowWiningAnimations()
    {
        //Player winning and losing animations
        //Focus camera on winning players field to show them animating

    }

    [Button]
    public void IncreaseScoreFor(Team team)
    {
        IncreaseScoreFor((team == Team.A) ? 0 : 1);
        //SetState(State.AwardingPoints);

    }

    [Button]
    public void IncreaseScoreFor(int playerId)
    {
        if (Provider.Instance.HasStateAuthority)
        {
            this.matchInfo.AddScore(playerId);

        }

    }

    private void ResetCourtState()
    {
        Provider.Instance.GameNetworking.ResetPlayerPositions();

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

    internal void NotifyPlayerWon(MatchInfo.ScoreData scoreData)
    {
        this.winningScore = scoreData;
        Provider.Instance.GameState.SetState(GameState.State.WinResultsCheck);

    }

    public void SetMatchInfo(MatchInfo matchInfo)
    {
        this.matchInfo = matchInfo;

        matchInfo.PlayerWonEvent.AddListener(NotifyPlayerWon);

    }

    [Serializable]
    public enum State
    {
        Menu,
        StartMatch,
        RestartMatch,
        SetStart,
        RallyStart,
        AwardingPoints,
        SetFinished,
        WinResultsCheck,
        MatchEnded,
        AbortMatch
    }

    [Serializable]
    public enum Team
    {
        A,
        B
    }

    [Serializable]
    public enum Mode
    {
        Host,
        Client
    }

}

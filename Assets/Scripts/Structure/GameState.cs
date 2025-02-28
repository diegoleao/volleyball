using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameState : MonoBehaviour
{

    [SerializeField] MainMenuScreen MainMenu;

    private State state;

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

    public void RestartMatch()
    {
        Provider.Instance.GameNetworking.RestartMatch();

    }

    public void SetState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.Menu:
                Instantiate(MainMenu);
                break;

            case State.StartMatch:
                ResetMatch();
                SetState(State.ResetCourtState);
                break;

            case State.ResetCourtState:
                //  - return players to their reset positions
                //  - remove ball, etc
                //  - let characters move and main game loop play out
                SetState(State.SpawnBall);
                break;

            case State.SpawnBall:
                //Provider.Instance.BallSpawner.Spawn();
                break;

            case State.AwardingPoints:
                // Lock players positions
                // Show any "score!" animation
                // await "MatchInfo.AddPointTo" data to be propagated and only then:
                SetState(State.ResetCourtState);
                break;

            case State.GameEnded:
                //if 3 games already ended:
                //  SetState(GameStates.MatchEnded);
                //else
                //   Show timed congratulatory message
                //***4 seconds later
                break;

            case State.MatchEnded:
                //Show end screen with results
                //Move players to winning positions
                //Move camera there
                //Player winning and losing animations
                //***Wait for a few seconds
                SetState(State.FinalResultCheck);
                break;

            case State.FinalResultCheck:
                //Show prompt to return to main menu
                break;

        }

    }

    [Button]
    public void IncreaseScoreFor(Team team)
    {
        GameState.IncreaseScoreFor((team == Team.A) ? 0 : 1);
        //SetState(State.AwardingPoints);

    }

    [Button]
    public static void IncreaseScoreFor(int playerId)
    {
        if (Provider.Instance.HasStateAuthority)
        {
            Provider.Instance.GameNetworking.MatchInfo.AddScore(playerId);
        }

    }

    private void ResetMatch()
    {
        //HudView?.Close();
        SetState(State.ResetCourtState);
    }

    [Serializable]
    public enum State
    {
        Menu,
        StartMatch,
        SpawnBall,
        ResetCourtState,
        AwardingPoints,
        GameEnded,
        FinalResultCheck,
        MatchEnded,
        Exit
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

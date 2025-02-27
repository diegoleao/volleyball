using Fusion;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameState : MonoBehaviour
{

    [SerializeField] Menu Menu;

    [SerializeField] HudView HudView;

    private State state;

    public void Initialize()
    {
        SetState(State.Menu);
    }

    public void StartMatch(GameMode mode)
    {
        Provider.Instance.BasicSpawner.StartNetwork(mode, () =>
        {
            SetState(State.StartMatch);
        });
        
    }

    public void SetState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.Menu:
                Instantiate(Menu);
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
    public void AddPointTo(int playerId)
    {
        if (Provider.Instance.HasStateAuthority)
        {
            Provider.Instance.BasicSpawner.MatchInfo.AddScore(playerId);
        }
        //SetState(State.AwardingPoints);

    }

    private void ResetMatch()
    {
        //HudView?.Close();
        Instantiate(HudView);
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
    public enum Mode
    {
        Host,
        Client
    }

}

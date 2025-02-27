using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{

    public MatchInfo MatchInfo;

    [SerializeField] Canvas Menu;
    State state;

    public void SetState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.Menu:
                Instantiate(Menu);
                break;

            case State.StartMatch:
                FindAnyObjectByType<Menu>().Close();
                this.MatchInfo = new MatchInfo();
                SetState(State.ResetCourtState);
                break;

            case State.ResetCourtState:
                //  - return players to their reset positions
                //  - remove ball, etc
                //  - let characters move and main game loop play out
                SetState(State.SpawnBall);
                break;

            case State.SpawnBall:
                Provider.Instance.BallSpawner.Spawn();
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

    public void AddPointTo(int playerId)
    {
        this.MatchInfo.AddPointTo(playerId);
        SetState(State.AwardingPoints);

    }

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

}

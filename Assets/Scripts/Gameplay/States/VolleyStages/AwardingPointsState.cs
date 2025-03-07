
using UnityEngine;

public class AwardingPointsState : BaseState
{

    public override void OnEnter()
    {
        if (GameState.ResetPlayerPositionOnScore)
        {
            Provider.API.ResetPlayerPositions();
        }

        // Lock players positions
        // Show any "score!" animation
        // await "MatchInfo.AddPointTo" data to be propagated and only then:

        if (!this.GameState.LocalMatchInfo.IsMatchFinished)
        {
            StateMachine.QueueNext<RallyStartState>();
#if UNITY_EDITOR
            Debug.Log($"Match still ongoing at " +
                      $"A: {this.GameState.LocalMatchInfo.Scores[0].score} vs " +
                      $"B: {this.GameState.LocalMatchInfo.Scores[1].score}.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"Match finished, skipping Rally Start. Final Score " +
                      $"A: {this.GameState.LocalMatchInfo.Scores[0].score} vs " +
                      $"B: {this.GameState.LocalMatchInfo.Scores[1].score}.");
#else
                    Debug.Log("Match finished, Skipping Rally Start.");
#endif
        }
    }
    public override void OnCreate()
    {

    }

    public override void OnExit()
    {

    }

    public override void StateUpdate()
    {

    }

}

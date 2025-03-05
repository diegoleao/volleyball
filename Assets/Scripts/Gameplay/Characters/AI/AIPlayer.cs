
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour, IPlayer
{

    [Header("AI Configuration")]
    [SerializeField] float ballFollowDistance = 10;
    [SerializeField] float ballHitDistance = 3;

    public Team Team { get; private set; }
    public bool IsAI { get; private set; }

    private BallHitting ballHitting;
    private JumpComponent jumpComponent;
    private AIMovement aiMovement;
    private IVolleyball currentVolleyball;
    private Vector3 currentVelocity;
    private float ballDistance;

    [Button]
    public void DebugInitialization()
    {
        Initialize(Team.A, isAI: true);

    }

    public void Initialize(Team team, bool isAI)
    {
        this.jumpComponent = GetComponent<JumpComponent>();
        this.jumpComponent.Initialize();

        this.ballHitting = GetComponent<BallHitting>();
        this.ballHitting.Initialize(team, isAI);

        this.aiMovement = GetComponent<AIMovement>();
        this.aiMovement.Initialize();

        this.Team = team;
        this.IsAI = isAI;

    }

    public void InjectVolleyball(IVolleyball currentVolleyball)
    {
        this.currentVolleyball = currentVolleyball;
        this.aiMovement.InjectVolleyball(this.currentVolleyball);

    }

    void Update()
    {
        ballDistance = Vector3.Distance(this.transform.position, currentVolleyball.Position);

        if (IsBallNearby())
        {
            MoveTowardsTheBall();
        }

    }

    private void MoveTowardsTheBall()
    {

        if (IsBallWithinHitDistance())
        {
            this.aiMovement.UpdateBallDirection(forceStop: true);
            ballHitting.HitTheBall();

        }
        else
        {
            this.aiMovement.UpdateBallDirection();

        }

    }

    private bool IsBallWithinHitDistance()
    {
        return ballDistance < ballHitDistance;
    }

    private bool IsBallNearby()
    {
        return ballDistance < ballFollowDistance;
    }

}
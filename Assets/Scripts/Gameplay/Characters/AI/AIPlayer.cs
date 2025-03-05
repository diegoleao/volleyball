
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
        Initialize(Team.A);

    }

    public void Initialize(Team team)
    {
        this.jumpComponent = GetComponent<JumpComponent>();
        this.jumpComponent.Initialize();

        this.ballHitting = GetComponent<BallHitting>();
        this.ballHitting.Initialize(team);

        this.aiMovement = GetComponent<AIMovement>();
        this.aiMovement.Initialize();

        this.Team = team;

    }

    public void InjectVolleyball(IVolleyball currentVolleyball)
    {
        this.currentVolleyball = currentVolleyball;
        this.aiMovement.InjectVolleyball(this.currentVolleyball);

    }

    void Update()
    {
        if (currentVolleyball == null)
            return;

        ballDistance = Vector3.Distance(this.transform.position, currentVolleyball.Position);

        if (IsBallNearby())
        {
            MoveTowardsTheBall();
        }
        else
        {
            this.aiMovement.UpdateMovementDirection(isBallInSight: false);
        }

    }

    private void MoveTowardsTheBall()
    {

        if (IsBallWithinHitDistance())
        {
            ballHitting.HitTheBall();
            this.aiMovement.ForceStop();
        }
        else
        {
            this.aiMovement.UpdateMovementDirection(isBallInSight: true);

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
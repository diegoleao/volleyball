
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{

    [Header("AI Configuration")]
    [SerializeField] float ballFollowDistance = 10;
    [SerializeField] float ballHitDistance = 3;

    public Team Team { get; private set; }
    public bool IsAI { get; private set; }

    private BallHitting ballHitting;
    private JumpComponent jumpComponent;
    private AIMovement aiMovement;
    private LocalVolleyball currentVolleyball;
    private Vector3 currentVelocity;
    private float ballDistance;

    [Button]
    public void DebugInitialization()
    {
        Initialize(Team.A);

    }

    public void Initialize(Team team)
    {
        this.Team = team;

        this.jumpComponent = GetComponent<JumpComponent>();
        this.jumpComponent.Initialize();

        this.ballHitting = GetComponent<BallHitting>();
        this.ballHitting.Initialize(this.Team);

        this.aiMovement = GetComponent<AIMovement>();
        this.aiMovement.Initialize(this.Team);


    }

    public void InjectVolleyball(BaseVolleyball currentVolleyball)
    {
        this.currentVolleyball = (LocalVolleyball)currentVolleyball;
        this.aiMovement.InjectVolleyball(this.currentVolleyball);

    }

    void Update()
    {
        if (currentVolleyball == null)
            return;

        ballDistance = Vector3.Distance(this.transform.position, currentVolleyball.Position);

        if (IsBallNearby())
        {
            if (IsBallWithinHitDistance())
            {
                this.aiMovement.ForceStop();
                this.aiMovement.FaceOtherCourtImmediately();
                ballHitting.HitTheBall();
            }
            else
            {
                this.aiMovement.UpdateMovementDirection(isBallInSight: true);

            }
        }
        else
        {
            this.aiMovement.UpdateMovementDirection(isBallInSight: false);
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
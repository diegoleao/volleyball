
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] bool isDebugging;

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
        Provider.Register<AIPlayer>(this);

    }

    public void InjectVolleyball(LocalVolleyball currentVolleyball)
    {
        this.currentVolleyball = currentVolleyball;
        this.aiMovement.InjectVolleyball(this.currentVolleyball);

    }

    void Update()
    {
        if (currentVolleyball == null)
        {
            this.aiMovement.MovementUpdate(isBallInSight: false);
            return;
        }

        ballDistance = Vector3.Distance(this.transform.position, currentVolleyball.Position);

        if (CanFollowBall())
        {
            if (InHitDistance())
            {
                Log($"[InHitDistance] {ballDistance}");
                this.aiMovement.StopToHitTheBall();
                ballHitting.HitTheBall();
            }
            else
            {
                Log($"[CanFollowBall] {ballDistance}");
                this.aiMovement.MovementUpdate(isBallInSight: true);
            }
        }
        else
        {
            this.aiMovement.MovementUpdate(isBallInSight: false);
        }

    }

    private bool InHitDistance()
    {
        return ballDistance < ballHitDistance;
    }

    private bool CanFollowBall()
    {
        return ballDistance < ballFollowDistance;
    }
    private void Log(string message)
    {
#if UNITY_EDITOR
        if (isDebugging)
            Debug.Log($"[AI][Player] {message}");
#endif

    }

}
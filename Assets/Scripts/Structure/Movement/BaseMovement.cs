using System;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    [SerializeField] protected float speed = 10;

    [SerializeField] protected float rotationSpeed = 10f;

    protected Rigidbody rb;

    protected bool isInitialized;

    protected Vector3 moveDirection;

    protected Vector3 currentVelocity;

    protected Vector3 courtCenter;

    protected Vector3 spawnCenter;

    public virtual void Initialize(Team team)
    {
        this.rb = GetComponent<Rigidbody>();
        courtCenter = Provider.Instance.CourtCenter.position;
        spawnCenter = Provider.Instance.CourtTriggers.GetTeamSpawnPosition(team, transform.position.y);
        isInitialized = true;

    }

    void Update()
    {
        if (!isInitialized)
            return;

        UpdateMovement();

    }

    protected virtual void UpdateMovement() { }

    public void FaceOtherCourtImmediately()
    {
        moveDirection = courtCenter - this.transform.position;
        moveDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.MoveRotation(targetRotation);

    }

    private void FixedUpdate()
    {
        if (!isInitialized)
            return;

        currentVelocity = rb.velocity;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            rb.velocity = new Vector3(moveDirection.x * speed, currentVelocity.y, moveDirection.z * speed);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

        }
        else
        {
            rb.velocity = new Vector3(0, currentVelocity.y, 0);

        }

    }

}

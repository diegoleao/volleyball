﻿using System;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    [SerializeField] protected float speed = 10;

    [SerializeField] protected float rotationSpeed = 10f;

    protected Rigidbody rb;

    protected bool isInitialized;

    protected Vector3 moveDirection;

    protected Vector3 rotationDirection;

    protected Vector3 currentVelocity;

    protected Vector3 netCenter;

    protected Vector3 spawnCenter;

    public virtual void Initialize(Team team)
    {
        this.rb = GetComponent<Rigidbody>();
        netCenter = Provider.CourtCenter.position;
        spawnCenter = Provider.CourtTriggers.GetSpawnCenter(team);
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
        rotationDirection = netCenter - this.transform.position;
        rotationDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
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

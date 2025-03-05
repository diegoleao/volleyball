using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpComponent : MonoBehaviour
{

    [SerializeField] bool isAI;
    [SerializeField] float jumpForce;

    private bool isGrounded;
    private Rigidbody rb;
    private JoystickSingleplayer joystickSingleplayer;
    private bool initialized;
    private bool jumpQueued;

    public void Initialize()
    {
        initialized = true;
        rb = GetComponent<Rigidbody>();
        joystickSingleplayer = GetComponent<JoystickSingleplayer>();

    }

    void Update()
    {
        if (!initialized || isAI)
            return;

        if (joystickSingleplayer.GetJumpButtonDown() && isGrounded)
        {
            ExecuteJump();
        }

    }

    public void ExecuteJump()
    {
        jumpQueued = true;

    }

    private void FixedUpdate()
    {
        if (jumpQueued)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpQueued = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

    }

}

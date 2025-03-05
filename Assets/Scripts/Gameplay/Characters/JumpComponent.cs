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

    public void Initialize()
    {
        initialized = true;
        rb = GetComponent<Rigidbody>();
        joystickSingleplayer = GetComponent<JoystickSingleplayer>();

    }

    private bool queueJump;

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
        queueJump = true;

    }

    private void FixedUpdate()
    {
        if (queueJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            queueJump = false;
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

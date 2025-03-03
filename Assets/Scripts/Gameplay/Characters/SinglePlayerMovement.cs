using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JoystickSingleplayer))]
public class SinglePlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;

    private Rigidbody rb;

    private bool isInitialized;

    private Vector3 moveDirection;

    private JoystickSingleplayer joystickSingleplayer;

    [SerializeField] float rotationSpeed = 10f;

    private Vector3 forward;

    private Vector3 currentVelocity;

    public void Initialize()
    {
        isInitialized = true;
        rb = GetComponent<Rigidbody>();
        joystickSingleplayer = GetComponent<JoystickSingleplayer>();

    }

    void Update()
    {
        if (!isInitialized)
            return;

        moveDirection = new Vector3(joystickSingleplayer.Horizontal, 0, joystickSingleplayer.Vertical);

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

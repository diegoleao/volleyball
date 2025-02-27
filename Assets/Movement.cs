using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    float horizontal;
    float vertical;
    Vector3 moveDirection;
    Rigidbody rb;
    [SerializeField] float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical);

    }

    private void FixedUpdate()
    {
        rb.MovePosition(this.transform.position + moveDirection.normalized * speed * Time.fixedDeltaTime);

    }

}

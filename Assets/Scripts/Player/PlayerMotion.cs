using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    public float speed = 8f;
    public float gravity = -9.8f;
    private bool isOnGround = false;
    private Vector3 velocity;
    private CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        if (isOnGround)
            velocity.y = 0;
        controller.Move(velocity * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        isOnGround = controller.isGrounded;
    }
}

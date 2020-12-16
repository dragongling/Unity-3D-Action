﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float groundNormalLimit = 0.9f;

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    Vector3 velocity, desiredVelocity;

    Rigidbody body;

    bool desiredJump;
    bool onGround;
    

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        desiredJump |= Input.GetButtonDown("Jump");            
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;        
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }    

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= groundNormalLimit;
        }
    }

    private void FixedUpdate()
    {
        velocity = body.velocity;

        if (desiredJump)
        {
            desiredJump = false;
            if (onGround)
            {
                Jump();
            }            
        }        
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        onGround = false;
        body.velocity = velocity;
    }

    private void Jump()
    {
        velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
    }
}

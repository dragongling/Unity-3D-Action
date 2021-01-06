using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAirAcceleration = 10f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f;

    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [SerializeField, Min(0f)]
    float probeDistance = 1f;

    [SerializeField]
    LayerMask probeMask = -1;

    [SerializeField]
    Transform playerInputSpace = default;

    [SerializeField]
    bool groundCollisionDebug = false;

    Rigidbody body;
    Respawnable respawnable;

    Vector3 velocity;
    Vector3 desiredVelocity;
    Vector3 contactNormal;

    float minGroundDotProduct;
    int jumpPhase;
    bool desiredJump;
    int groundContactCount;
    int stepsSinceLastGrounded;
    int stepsSinceLastJump;

    bool OnGround => groundContactCount > 0;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        respawnable = GetComponent<Respawnable>();
        OnValidate();
    }

    void Update()
    {
        if (!PauseControl.gameIsPaused)
        {
            Vector2 playerInput;
            if (Input.GetButtonDown("Respawn") && respawnable != null)
            {
                respawnable.Respawn();
            }
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            desiredJump |= Input.GetButtonDown("Jump");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);

            if (playerInputSpace)
            {
                Vector3 forward = playerInputSpace.forward;
                forward.y = 0f;
                forward.Normalize();
                Vector3 right = playerInputSpace.right;
                right.y = 0f;
                right.Normalize();
                desiredVelocity =
                    (forward * playerInput.y + right * playerInput.x) * maxSpeed;
            }
            else
            {
                desiredVelocity =
                    new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
            }

            if (groundCollisionDebug)
            {
                GetComponent<Renderer>().material.SetColor(
                    "_Color", OnGround ? Color.black : Color.white
                );
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        body.velocity = velocity;
        AdjustRotation();
        ClearState();
    }

    private void AdjustRotation()
    {
        transform.Rotate(body.rotation.eulerAngles);
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }    

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount++;
                contactNormal += normal;
            }
        }
    }

    private void UpdateState()
    {
        stepsSinceLastGrounded++;
        stepsSinceLastJump++;
        velocity = body.velocity;
        if (OnGround || SnapToGround())
        {
            stepsSinceLastGrounded = 0;
            jumpPhase = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }    

    private void Jump()
    {
        if(OnGround || jumpPhase < maxAirJumps)
        {
            stepsSinceLastJump = 0;
            jumpPhase++;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }        
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
        {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }
        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask))
        {
            return false;
        }
        if (hit.normal.y < minGroundDotProduct)
        {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        return true;
    }
}

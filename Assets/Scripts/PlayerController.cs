using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float jumpPower = 6f;
    public float jumpSurfaceAngle = 89f; // The angle of the surface the player is allowed to jump off of
    public float airControlFactor = 0.8f; // Determines how much control the player has in the air
    public float maxJumpVelocity = .2f; // Maximum upward velocity from a jump
    public float maxAngularVelocity = 20f; // This is the max speed that the marble can rotate
    public float torqueAmount = 2f; // Adjust this value to increase/decrease the rotation speed
    public float additionalGravityForce = 0f; // Extra downward force
    public float stickinessForce = 20f;


    private bool jump;
    private bool isGrounded;
    private bool groundContact; // Checking for contact with a ground surface
    private bool wallContact; // Checking for contact with a wall surface
    private Vector3 jumpDirection;
    private Vector3 moveDirection;
    private float moveX;
    private float moveZ;


    private Rigidbody rb;
    private Camera cam;


    private PlayerSizeManager sizeManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpDirection = Vector3.up;
        cam = Camera.main;

        // Remove the cap on the maximum angular velocity to allow for faster rotation
        rb.maxAngularVelocity = maxAngularVelocity;

        sizeManager = GetComponent<PlayerSizeManager>();
    }

    void Update() //Use for input processing
    {
        HandleInput();
        //SurfaceAlignment();
    }

    private void FixedUpdate() //Use for movement
    {
        EvaluateJumpPermission();
        Move();
        Jump();
        ApplyAirResistance();
        ApplyManualFriction();
        ApplyAdditionalGravity();
        ApplyStickiness();
    }
    
    private void ApplyStickiness()
    {
        if (isGrounded)
        {
            // Apply a force towards the ground to simulate stickiness
            rb.AddForce(-jumpDirection * stickinessForce, ForceMode.Force);
        }
    }

    private void ApplyAdditionalGravity()
    {
        // Apply additional gravity when the marble is grounded to help it stick to surfaces
        rb.AddForce(Vector3.down * additionalGravityForce, ForceMode.Force);
    }

    private void ApplyManualFriction()
    {
        if (isGrounded)
        {
            // Get the velocity parallel to the ground
            Vector3 groundVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Calculate friction force that opposes the motion
            Vector3 frictionForce = -groundVelocity.normalized * moveSpeed * Time.fixedDeltaTime;

            // Apply friction force
            rb.AddForce(frictionForce, ForceMode.Force);
        }
    }

    private void ApplyAirResistance()
    {
        // Only apply air resistance when the marble is not grounded and is moving horizontally
        if (!isGrounded)
        {
            // Calculate a damping force based on the current horizontal velocity
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 airResistance = -horizontalVelocity * moveSpeed * Time.fixedDeltaTime;

            // More realistic air resistance was testing
            // airResistance = -horizontalVelocity * horizontalVelocity.magnitude * moveSpeed * Time.fixedDeltaTime;

            // Apply the air resistance force
            rb.AddForce(airResistance, ForceMode.Force);
        }
    }

    private void HandleInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        jump = Input.GetButton("Jump");

        // Calculate the direction from the camera to the player
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        // Make sure the movement is aligned with the ground plane, not the camera tilt
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate the moveDirection relative to camera orientation
        // Also normalize the direction so that moving diagonally does not speed up the player
        moveDirection = (forward * moveZ + right * moveX).normalized;
    }
     // Force and torque to move the marble
    private void Move()
    {
        // Determine the amount of control the player has over movement (1 when grounded, less when in the air)
        float controlFactor = isGrounded ? 1f : airControlFactor;

        Vector3 moveForce = moveDirection * (moveSpeed * controlFactor);
        rb.AddForce(moveForce, ForceMode.Force);

        float inputMagnitude = new Vector2(moveX, moveZ).magnitude;

        Vector3 torqueDirection = Vector3.Cross(Vector3.up, moveDirection).normalized;

        Vector3 torqueForce = torqueDirection * torqueAmount * inputMagnitude;

        rb.AddTorque(torqueForce, ForceMode.Force);
    }

    private void Jump()
    {
        if (jump && groundContact)
        {
            // Apply the jump force
            rb.AddForce(jumpDirection * jumpPower, ForceMode.Impulse);
            
            // Clamp the velocity immediately after the jump to ensure it doesn't exceed the maximum
            // This is to make the marble stop jumping higher than normal after multiple jumps
            Vector3 velocity = rb.velocity;
            velocity.y = Mathf.Min(velocity.y, maxJumpVelocity);
            rb.velocity = velocity;

            // After jumping, reset groundContact and wallContact so it needs to be confirmed again
            groundContact = false;
            wallContact = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateGrounded(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateGrounded(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Assume not in contact with ground or wall when exiting collision
        groundContact = false;
        wallContact = false;
    }

    private void EvaluateGrounded(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle < jumpSurfaceAngle) // considered as ground
            {
                groundContact = true;
                jumpDirection = contact.normal; // Use the normal of the ground contact
            }
            else if (angle > jumpSurfaceAngle && angle < 180 - jumpSurfaceAngle) // considered as wall
            {
                wallContact = true;
            }
        }

        // Debug to check the contact states
        //Debug.Log("Ground contact: " + groundContact + ", Wall contact: " + wallContact);
    }

    // Call this in FixedUpdate before Jump to re-evaluate ability to jump
    private void EvaluateJumpPermission()
    {
        // Player can jump if there is a ground contact, regardless of wall contact
        isGrounded = groundContact && !wallContact;
    }
}
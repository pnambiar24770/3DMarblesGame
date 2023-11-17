//TODO

//FIX THE AIR RESISTANCE SO THE MARBLE DOES NOT HAVE SO MUCH CONTROL IN THE AIR
//MAKE THE MARBLE ROTATE MORE ACCURATELY
//ALLOW MARBLE MORE TORQUE IN THE AIR

//MAYBE TRY TO ADD MORE CUSTOM PHYSICS FOR THE MARBLE'S BOUNCINESS




using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpPower = 1f;
    public float jumpSurfaceAngle = 90f; // The angle of the surface the player is allowed to jump off of
    public float airControlFactor = 0.2f; // Determines how much control the player has in the air
    public float maxJumpVelocity = 10f; // Maximum upward velocity from a jump
    public float maxAngularVelocity = 20f;
    public float torqueAmount = 10f; // Adjust this value to increase/decrease the rotation speed
    public float additionalGravityForce = 50f; // Extra downward force
    public float stickinessForce = 100f; // New variable for the stickiness effect


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


    //TESTING STICKYNESS
    /*public float rotationSpeed = 10f;
    public float maxRotationAngle = 30f;
    public LayerMask groundMask;
    [SerializeField]
    private AnimationCurve aniCurve;
    [SerializeField]
    private float Timer;*/




    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpDirection = Vector3.up;
        cam = Camera.main;

        // Remove the cap on the maximum angular velocity to allow for faster rotation
        rb.maxAngularVelocity = maxAngularVelocity; // Or set it to a high enough value that won't limit your marble
    }

    void Update() //Use for input processing
    {
        HandleInput();


        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit info = new RaycastHit();
        Quaternion rf = Quaternion.Euler(0, 0, 0);

        /*if (Physics.Raycast(ray, out info, groundMask))
        {

            //  rf = Quaternion.Lerp(transform.rotation , Quaternion.FromToRotation(Vector3.up, info.normal), aniCurve.Evaluate(Timer));
            //  transform.rotation = Quaternion.Euler(rf.eulerAngles.x, transform.eulerAngles.y,rf.eulerAngles.z);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, info.normal), aniCurve.Evaluate(Timer));

        }*/
    }

    private void FixedUpdate() //Use for movement
    {
        EvaluateJumpPermission();
        Move();
        Jump();
        ApplyAirResistance();
        ApplyManualFriction();
        /*ApplyAdditionalGravity();
        ApplyStickiness();*/
    }

    /*private void ApplyStickiness()
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
    }*/

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

    private void Move()
    {
        // Determine the amount of control the player has over movement (1 when grounded, less when in the air)
        float controlFactor = isGrounded ? 1f : airControlFactor;

        // Calculate the desired move force and apply it
        Vector3 moveForce = moveDirection * (moveSpeed * controlFactor);
        rb.AddForce(moveForce, ForceMode.Force);

        // Calculate input magnitude which can be used to scale the torque for smoother rotation
        float inputMagnitude = new Vector2(moveX, moveZ).magnitude;

        // Calculate the desired torque direction (in world space) for the marble to roll in the move direction
        Vector3 torqueDirection = Vector3.Cross(Vector3.up, moveDirection).normalized;

        // Calculate the torque force, scaled by the input magnitude and the current speed of the marble
        Vector3 torqueForce = torqueDirection * torqueAmount * inputMagnitude;

        // Apply the torque force smoothly
        rb.AddTorque(torqueForce, ForceMode.Force);
    }

    private void Jump()
    {
        if (jump && groundContact)
        {
            // Apply the jump force
            rb.AddForce(jumpDirection * jumpPower, ForceMode.Impulse);
            
            // Clamp the velocity immediately after the jump to ensure it doesn't exceed the maximum
            Vector3 velocity = rb.velocity;
            velocity.y = Mathf.Min(velocity.y, maxJumpVelocity);
            rb.velocity = velocity;

            // After jumping, reset groundContact so it needs to be confirmed again
            groundContact = false;
            wallContact = false; // Also reset the wallContact to ensure it's recalculated
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        jumpDirection = collision.contacts[0].normal;
    }*/

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
            if (angle < jumpSurfaceAngle) // Contact is considered as ground
            {
                groundContact = true;
                jumpDirection = contact.normal; // Use the normal of the ground contact
            }
            else if (angle > jumpSurfaceAngle && angle < 180 - jumpSurfaceAngle) // Contact is considered as wall
            {
                wallContact = true;
            }
        }

        // Debug output to check the contact states
        Debug.Log("Ground contact: " + groundContact + ", Wall contact: " + wallContact);
    }

    // Call this in FixedUpdate before Jump to re-evaluate ability to jump
    private void EvaluateJumpPermission()
    {
        // Player can jump if there is a ground contact, regardless of wall contact
        isGrounded = groundContact && !wallContact;
    }
}
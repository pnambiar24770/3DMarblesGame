

//TODO

//FIX THE AIR RESISTANCE SO THE MARBLE DOES NOT HAVE SO MUCH CONTROL IN THE AIR
//MAKE IT SO THE MARBLE CAN JUMP WHEN AGAINST THE WALL, THE ISGROUNDED BOOL IS FALSE WHEN CONTACTING BOTH THE WALL AND THE GROUND
//FIX THE JUMP FROM MULTIPLYING WITH THE BOUNCINESS FROM THE PHYSICS MATERIAL, THE JUMPING IS INCONSISTENT, IT WORKS FINE WITHOUT THE PHYSICS MATERIAL
//STOP THE MARBLE FROM SKATING SO MUCH ON THE GROUND
//EXPERIMENT WITH THE MARBLES WEIGHT FOR MORE SATISFYING CONTROLS
//MAKE THE MARBLE ROTATE MORE ACCURATELY

//MAYBE TRY TO ADD MORE CUSTOM PHYSICS FOR THE MARBLE'S BOUNCINESS




using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpPower = 1f;
    public float jumpSurfaceAngle = 90f; // The angle of the surface the player is allowed to jump off of
    public float airControlFactor = 0.2f; // Determines how much control the player has in the air

    private bool jump;
    private bool isGrounded;
    private bool groundContact;
    private bool wallContact;
    private Vector3 jumpDirection;
    private Vector3 moveDirection;
    private float moveX;
    private float moveZ;

    private Rigidbody rb;
    private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpDirection = Vector3.up;
        cam = Camera.main;
    }

    void Update() //Use for input processing
    {
        HandleInput();
    }

    private void FixedUpdate() //Use for movement
    {
        EvaluateJumpPermission();
        Move();
        Jump();
        ApplyAirResistance();
    }

    private void ApplyAirResistance()
    {
        // Only apply air resistance when the marble is not grounded and is moving horizontally
        if (!isGrounded)
        {
            // Calculate a damping force based on the current horizontal velocity
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 airResistance = -horizontalVelocity * moveSpeed * Time.fixedDeltaTime;

            // Optionally, you can make the air resistance relative to the velocity squared for more realism
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

        // Make sure the movement is aligned with the ground plane, not the camera's tilt
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate the moveDirection relative to camera's orientation
        // Also normalize the direction so that moving diagonally does not speed up the player
        moveDirection = (forward * moveZ + right * moveX).normalized;
    }

    private void Move()
    {
        // Use air control factor only if the marble is in the air
        float controlFactor = isGrounded ? 1f : airControlFactor;

        // Apply a force to move the marble
        rb.AddForce(moveDirection * (moveSpeed * controlFactor), ForceMode.Force);
    }

    private void Jump()
    {
        if (jump && groundContact)
        {
            rb.AddForce(jumpDirection * jumpPower, ForceMode.Impulse);
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
        groundContact = false;
        wallContact = false;

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
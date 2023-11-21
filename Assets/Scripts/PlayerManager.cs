//TODO

//Adjust the sofybody marble physics more
//


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject ballObject; // Reference to the GameObject with softbody bones
    public SoftbodyToggle softbodyToggle;
    public PhysicMaterial physicsMaterial; // Reference to the Physics Material
    public SphereCollider sphereCollider;


    public enum BallSize
    {
        Small,
        Medium,
        Large,
        Softbody
    }

    public BallSize currentSize = BallSize.Medium;

    //physics for each size
    public float smallSizeMoveSpeed = 10f;
    public float smallGravity = -5f;
    public float smallStickinessForce = 5f;
    public float smallTorque = 2f;
    public float smallDynamicFriction = 0f;
    public float smallBounciness = .97f;

    public float mediumSizeMoveSpeed = 30f;
    public float mediumGravity = 0;
    public float mediumStickinessForce = 15f;
    public float mediumTorque = 2f;
    public float mediumDynamicFriction = 0f;
    public float mediumBounciness = .97f;

    public float largeSizeMoveSpeed = 20f;
    public float largeGravity = 10f;
    public float largeStickinessForce = 25f;
    public float largeTorque = 2f;
    public float largeDynamicFriction = 0f;
    public float largeBounciness = .97f;

    public float softbodyMoveSpeed = 10f;
    public float softbodyGravity = -5f;
    public float softbodyStickinessForce = 40f;
    public float softbodyTorque = 20f;
    public float softbodyDynamicFriction = 2f;
    public float softbodyBounciness = 0f;
    public float softbodyRadius = .5f;

    [SerializeField] private float rayLengthMultiplier = 4.0f;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        UpdatePhysicsParameters();

        // Ignore collision between the player's collider and the bones
        Collider playerCollider = GetComponent<Collider>();
        if (ballObject != null)
        {
            Collider[] boneColliders = ballObject.GetComponentsInChildren<Collider>();
            foreach (var boneCollider in boneColliders)
            {
                Physics.IgnoreCollision(playerCollider, boneCollider);
            }
        }
    }

    void Update()
    {
        HandleInput();
    }


    float GetCurrentSizeMultiplier()
    {
        switch (currentSize)
        {
            case BallSize.Small:
                return 0.125f;
            case BallSize.Medium:
                return .5f;
            case BallSize.Large:
                return 2f;
            default:
                return .5f; // Default toj medium size
        }
    }

    void OnDrawGizmos()
    {
        float raycastDistance = rayLengthMultiplier * GetCurrentSizeMultiplier();
        Vector3 ballPosition = transform.position;

        // Gizmo for each pair of raycasts (check the CanChangeSize() method for the actual raycasts)
        Vector3[][] oppositeRayPairs = {
            new Vector3[] { Vector3.up, -Vector3.up },
            new Vector3[] { Vector3.left, Vector3.right },
            new Vector3[] { Vector3.forward, -Vector3.forward },
            new Vector3[] { Quaternion.Euler(0, 135, 0) * Vector3.forward, Quaternion.Euler(0, -45, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(0, 45, 0) * Vector3.forward, Quaternion.Euler(0, -135, 0) * Vector3.forward },
            /*new Vector3[] { Quaternion.Euler(45, 135, 0) * Vector3.forward, Quaternion.Euler(-45, -45, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(45, 45, 0) * Vector3.forward, Quaternion.Euler(-45, -135, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(-45, 45, 0) * Vector3.forward, Quaternion.Euler(45, -135, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(-45, 135, 0) * Vector3.forward, Quaternion.Euler(45, -45, 0) * Vector3.forward },*/
        };

        // Colors to keep track of each pair of raycasts
        Color[] gizmoColors = {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.white,
            Color.gray,
            Color.black,
        };

        // Loop through each pair and give them a color
        for (int i = 0; i < oppositeRayPairs.Length; i++)
        {
            // Set the color for this pair
            Gizmos.color = gizmoColors[i];

            // Draw the first ray in the pair
            Vector3 rayDirection1 = oppositeRayPairs[i][0];
            Gizmos.DrawRay(ballPosition, rayDirection1 * raycastDistance);

            // Draw the second ray in the pair
            Vector3 rayDirection2 = oppositeRayPairs[i][1];
            Gizmos.DrawRay(ballPosition, rayDirection2 * raycastDistance);
        }
    }

    // This is probably way overly complicated to stop the ball from growing in certain conditions but idk
    // The commented out pairs are probably too restrictive in allowing the player to grow, will have to reasses 
    private bool CanChangeSize(bool shrink)
    {
        float raycastDistance = rayLengthMultiplier * GetCurrentSizeMultiplier();
        Vector3 ballPosition = transform.position;

        // Define pairs of opposite ray directions
        Vector3[][] oppositeRayPairs = {
            new Vector3[] { Vector3.up, -Vector3.up },
            new Vector3[] { Vector3.left, Vector3.right },
            new Vector3[] { Vector3.forward, -Vector3.forward },
            new Vector3[] { Quaternion.Euler(0, 135, 0) * Vector3.forward, Quaternion.Euler(0, -45, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(0, 45, 0) * Vector3.forward, Quaternion.Euler(0, -135, 0) * Vector3.forward },
            /*new Vector3[] { Quaternion.Euler(45, 135, 0) * Vector3.forward, Quaternion.Euler(-45, -45, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(45, 45, 0) * Vector3.forward, Quaternion.Euler(-45, -135, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(-45, 45, 0) * Vector3.forward, Quaternion.Euler(45, -135, 0) * Vector3.forward },
            new Vector3[] { Quaternion.Euler(-45, 135, 0) * Vector3.forward, Quaternion.Euler(45, -45, 0) * Vector3.forward },*/
        };

        // Count the number of pairs with both raycasts blocked
        int blockedPairCount = 0;

        for (int i = 0; i < oppositeRayPairs.Length; i++)
        {
            // Check the first ray in the pair
            Vector3 rayDirection1 = oppositeRayPairs[i][0];
            bool isBlocked1 = Physics.Raycast(ballPosition, rayDirection1, raycastDistance);

            // Check the second ray in the pair
            Vector3 rayDirection2 = oppositeRayPairs[i][1];
            bool isBlocked2 = Physics.Raycast(ballPosition, rayDirection2, raycastDistance);

            // If both rays in the pair are blocked, increment the blocked pair count
            if (isBlocked1 && isBlocked2)
            {
                blockedPairCount++;
            }
        }

        // If any pair of opposite rays is blocked and we're trying to grow, prevent size change
        // This is to try to stop the marble from clipping into objects/terrain when trying to grow
        if (blockedPairCount > 0 && !shrink)
        {
            return false;
        }

        return true;
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left click to increase size
        {
            IncreaseSize();
        }
        else if (Input.GetMouseButtonDown(1)) // Right click to decrease size
        {
           DecreaseSize();
        }
    }

    void IncreaseSize()
    {
        // Check if it's allowed to increase size
        if (CanChangeSize(false))
        {
            switch (currentSize)
            {
                case BallSize.Small:
                    currentSize = BallSize.Medium;
                    break;
                case BallSize.Medium:
                    if (softbodyToggle.softBodyActive == false)
                    {
                        currentSize = BallSize.Large;
                    }
                    break;
            }

            UpdatePhysicsParameters();
        }
        else
        {
            Debug.Log("Cannot increase size due to obstacles.");
        }
    }

    void DecreaseSize()
    {
        // Check if it's allowed to decrease size
        if (CanChangeSize(true))
        {
            switch (currentSize)
            {
                case BallSize.Large:
                    currentSize = BallSize.Medium;
                    break;
                case BallSize.Medium:
                    if (softbodyToggle.softBodyActive == false)
                    {
                        currentSize = BallSize.Small;
                    }
                    break;
            }

            UpdatePhysicsParameters();
        }
        else
        {
            Debug.Log("Cannot decrease size due to obstacles.");
        }
    }

    public void UpdatePhysicsParameters()
    {
        // Update physics parameters based on the current size
        switch (currentSize)
        {
            case BallSize.Small:
                playerController.moveSpeed = smallSizeMoveSpeed;
                playerController.additionalGravityForce = smallGravity;
                playerController.stickinessForce = smallStickinessForce;
                playerController.torqueAmount = smallTorque;
                physicsMaterial.dynamicFriction = smallDynamicFriction;
                physicsMaterial.bounciness = smallBounciness;
                sphereCollider.radius = .85f;
                break;
            case BallSize.Medium:
                playerController.moveSpeed = mediumSizeMoveSpeed;
                playerController.additionalGravityForce = mediumGravity;
                playerController.stickinessForce = mediumStickinessForce;
                playerController.torqueAmount = mediumTorque;
                physicsMaterial.dynamicFriction = mediumDynamicFriction;
                physicsMaterial.bounciness = mediumBounciness;
                sphereCollider.radius = .85f;
                break;
            case BallSize.Large:
                playerController.moveSpeed = largeSizeMoveSpeed;
                playerController.additionalGravityForce = largeGravity;
                playerController.stickinessForce = largeStickinessForce;
                playerController.torqueAmount = largeTorque;
                physicsMaterial.dynamicFriction = largeDynamicFriction;
                physicsMaterial.bounciness = largeBounciness;
                sphereCollider.radius = .85f;
                break;
            case BallSize.Softbody:
                playerController.moveSpeed = softbodyMoveSpeed;
                playerController.additionalGravityForce = softbodyGravity;
                playerController.stickinessForce = softbodyStickinessForce;
                playerController.torqueAmount = softbodyTorque;
                physicsMaterial.dynamicFriction = softbodyDynamicFriction;
                physicsMaterial.bounciness = softbodyBounciness;
                sphereCollider.radius = softbodyRadius;
                break;
        }
    }
}
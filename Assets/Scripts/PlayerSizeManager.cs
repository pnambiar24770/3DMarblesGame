using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSizeManager : MonoBehaviour
{
    public enum BallSize
    {
        Small,
        Medium,
        Large
    }

    public BallSize currentSize = BallSize.Medium;

    //physics for each size
    [SerializeField] private float smallSizeMoveSpeed = 10f;
    [SerializeField] private float mediumSizeMoveSpeed = 30f;
    [SerializeField] private float largeSizeMoveSpeed = 20f;

    [SerializeField] private float smallGravity = -5f;
    [SerializeField] private float mediumGravity = 0;
    [SerializeField] private float largeGravity = 10f;

    [SerializeField] private float smallStickinessForce = 5f;
    [SerializeField] private float mediumStickinessForce = 20f;
    [SerializeField] private float largeStickinessForce = 40f;

    [SerializeField] private float rayLengthMultiplier = 4.0f;


    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        UpdatePhysicsParameters();
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
                    currentSize = BallSize.Large;
                    break;
                    // Add additional cases for larger sizes if needed
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
                    currentSize = BallSize.Small;
                    break;
                    // Add additional cases for smaller sizes if needed
            }

            UpdatePhysicsParameters();
        }
        else
        {
            Debug.Log("Cannot decrease size due to obstacles.");
        }
    }

    void UpdatePhysicsParameters()
    {
        // Update physics parameters based on the current size
        switch (currentSize)
        {
            case BallSize.Small:
                playerController.moveSpeed = smallSizeMoveSpeed;
                playerController.additionalGravityForce = smallGravity;
                playerController.stickinessForce = smallStickinessForce;
                break;
            case BallSize.Medium:
                playerController.moveSpeed = mediumSizeMoveSpeed;
                playerController.additionalGravityForce = mediumGravity;
                playerController.stickinessForce = mediumStickinessForce;
                break;
            case BallSize.Large:
                playerController.moveSpeed = largeSizeMoveSpeed;
                playerController.additionalGravityForce = largeGravity;
                playerController.stickinessForce = largeStickinessForce;
                break;
                // Add additional cases for other physics parameters if needed
        }
    }
}

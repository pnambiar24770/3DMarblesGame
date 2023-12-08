using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public LineRenderer trajectoryLineRenderer;
    public float launchForce = 1000f;
    public Transform launchPoint; // Assign a child GameObject as the launch point
    public new AudioManager audio;
    public int resolution = 30;

    void Start()
    {
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.positionCount = resolution;
        }
    }

    void Update()
    {
        DrawTrajectory();
    }

    private void DrawTrajectory()
    {
        Vector3[] points = new Vector3[resolution];
        Vector3 startingPosition = launchPoint.position;
        Vector3 startingVelocity = launchPoint.forward * launchForce;

        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f; // Adjust time step for more accuracy
            points[i] = startingPosition + startingVelocity * time + Physics.gravity * time * time / 2f;
            if (points[i].y < startingPosition.y)
            {
                trajectoryLineRenderer.positionCount = i + 1;
                break;
            }
        }

        trajectoryLineRenderer.SetPositions(points);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            LaunchPlayer(other.gameObject);
        }
    }

    private void LaunchPlayer(GameObject player)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // Apply a force to the player's Rigidbody
            playerRb.AddForce(launchPoint.forward * launchForce, ForceMode.Impulse);
            audio.PlaySound("Blast");
        }
    }
}

using UnityEngine;

public class Bubbles : MonoBehaviour
{
    public GameObject player;
    public float horizontalMoveSpeed = 20f;  // Speed at which the bubble moves horizontally towards the player
    public float descentSpeed = 10f;          // Speed at which the bubble descends towards the player
    public float minimumVerticalDistance = 0.5f; // Minimum vertical distance to maintain above the player
    public float driftMagnitude = 0.1f;      // Magnitude of random horizontal drift

    private Transform playerTransform;
    private Vector3 driftDirection;

    void Start()
    {
        if (player != null)
        {
            playerTransform = player.GetComponent<Transform>();
        }
        driftDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Calculate the target position above the player
            Vector3 targetPosition = new Vector3(playerTransform.position.x, Mathf.Max(playerTransform.position.y + minimumVerticalDistance, transform.position.y - descentSpeed * Time.deltaTime), playerTransform.position.z);

            // Move the bubble horizontally towards the player
            Vector3 horizontalTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, horizontalTargetPosition, horizontalMoveSpeed * Time.deltaTime);

            // Descend towards the player
            transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);

            // Apply random horizontal drift
            transform.position += new Vector3(driftDirection.x, 0, driftDirection.z) * driftMagnitude * Time.deltaTime;
        }
    }
}

using UnityEngine;

public class Bubbles : MonoBehaviour
{
    public GameObject player;
    public float horizontalMoveSpeed = 20f;
    public float knockbackForce = 100;

    private Transform playerTransform;

    void Start()
    {
        if (player != null)
        {
            playerTransform = player.GetComponent<Transform>();
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Target position directly towards the player
            Vector3 targetPosition = playerTransform.position;

            // Move the bubble towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, horizontalMoveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            // Apply force to the player
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 forceDirection = (playerTransform.position - transform.position).normalized;
                playerRigidbody.AddForce(forceDirection * knockbackForce, ForceMode.Impulse);
            }

            Destroy(gameObject); // Destroy the bubble on contact
        }
    }
}

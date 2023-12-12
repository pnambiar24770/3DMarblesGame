using UnityEngine;

public class Bubbles : MonoBehaviour
{
    public GameObject player;
    public float horizontalMoveSpeed = 20f;
    public float descentSpeed = 10f;
    public float minimumVerticalDistance = 0.5f;
    public float driftMagnitude = 0.1f;

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
            Vector3 targetPosition = new Vector3(playerTransform.position.x, Mathf.Max(playerTransform.position.y + minimumVerticalDistance, transform.position.y - descentSpeed * Time.deltaTime), playerTransform.position.z);
            Vector3 horizontalTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, horizontalTargetPosition, horizontalMoveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
            transform.position += new Vector3(driftDirection.x, 0, driftDirection.z) * driftMagnitude * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            Destroy(gameObject); // Destroy the bubble on contact
        }
    }
}
using UnityEngine;


public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab; // Reference to the bubble prefab
    public float spawnInterval = 10f;
    public Vector3 spawnPosition;
    public GameObject player;

    private float timeSinceLastSpawn;

    void Start()
    {
        timeSinceLastSpawn = 0;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnBubble();
            timeSinceLastSpawn = 0;
        }
    }

    private void SpawnBubble() {
    GameObject bubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
    bubble.GetComponent<Bubbles>().player = player; 
}


}

using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    public GameObject enemyPrefab; // The enemy prefab to spawn
    public float spawnRadius = 10f; // Radius around center to spawn enemies
    public Vector3 spawnCenter = Vector3.zero; // Center point for random spawning
    
    [Header("Player Reference")]
    public GameObject playerObject; // Reference to the player to avoid spawning on top
    public float minDistanceFromPlayer = 2f; // Minimum distance to spawn from player
    
    [Header("Respawn Settings")]
    public float spawnDelay = 2f; // Delay before respawning
    
    private GameObject currentEnemy;
    private Enemy currentEnemyScript;
    private bool isRespawning = false; // Flag to prevent multiple spawns
    
    void Start()
    {
        // Auto-find player if not assigned
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogWarning("Player not found! Make sure the player GameObject has the 'Player' tag.");
            }
        }
        
        // Spawn the initial enemy
        SpawnEnemy();
    }
    
    void Update()
    {
        // Check if current enemy is dead or doesn't exist
        if ((currentEnemy == null || !currentEnemy.activeInHierarchy) && !isRespawning)
        {
            Debug.Log("Enemy is dead or missing! Respawning...");
            isRespawning = true;
            Invoke(nameof(SpawnEnemy), spawnDelay);
        }
    }
    

    void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            
            currentEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemyScript = currentEnemy.GetComponent<Enemy>();
            
            if (currentEnemyScript == null)
            {
                Debug.LogWarning("Enemy prefab does not have an Enemy script attached!");
            }
            
            isRespawning = false; // Reset flag after spawning
            Debug.Log("Enemy spawned at: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Enemy prefab is not assigned in the Inspector!");
        }
    }
    
 
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool validPosition = false;
        
        // Keep generating random positions until we find one that's not on the player
        while (!validPosition)
        {
            // Generate random position within spawn radius
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(0f, spawnRadius);
            
            randomPosition = spawnCenter + new Vector3(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance,
                0f
            );
            
            // Check if position is far enough from player
            if (playerObject != null)
            {
                float distanceToPlayer = Vector3.Distance(randomPosition, playerObject.transform.position);
                validPosition = distanceToPlayer >= minDistanceFromPlayer;
            }
            else
            {
                // If no player reference, just accept the position
                validPosition = true;
            }
        }
        
        return randomPosition;
    }
}

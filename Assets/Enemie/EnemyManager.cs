using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    

    [Header("Player Reference")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float minDistanceFromPlayer = 2f;

    [Header("Spawn rate")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int maxEnemies = 50;
    
    private int currentEnemyCount;

   

    private void Start()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogWarning("Player not found! Make sure the player GameObject has the 'Player' tag.");
            }
        }

        StartCoroutine(SpawnLoop());
    }
    
    
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            if (currentEnemyCount >= maxEnemies)
            return;
            
            Debug.LogError("Enemy prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemyCount++;
        
    }

    

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return Vector3.zero;
        }

        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (playerObject == null)
                return spawnPoint.position;

            if (Vector3.Distance(spawnPoint.position, playerObject.transform.position) >= minDistanceFromPlayer)
                return spawnPoint.position;
        }

        // If all spawn points are too close, just use a random one.
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }
}

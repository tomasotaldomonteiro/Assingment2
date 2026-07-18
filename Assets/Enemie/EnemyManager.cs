using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Vector3 spawnCenter = Vector3.zero;

    [Header("Player Reference")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float minDistanceFromPlayer = 2f;

    [Header("Respawn Settings")]
    [SerializeField] private float spawnDelay = 2f;

    private GameObject currentEnemy;
    private Enemy currentEnemyScript;
    private Coroutine respawnRoutine;

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

        SpawnEnemy();
    }

    private void OnDisable()
    {
        UnsubscribeCurrentEnemy();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned in the Inspector!");
            return;
        }

        if (currentEnemyScript != null)
        {
            currentEnemyScript.Died -= HandleEnemyDied;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        currentEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemyScript = currentEnemy.GetComponent<Enemy>();

        if (currentEnemyScript == null)
        {
            Debug.LogWarning("Enemy prefab does not have an Enemy script attached!");
            return;
        }

        currentEnemyScript.Died += HandleEnemyDied;
        Debug.Log("Enemy spawned at: " + spawnPosition);
    }

    private void HandleEnemyDied()
    {
        if (respawnRoutine != null)
        {
            StopCoroutine(respawnRoutine);
        }

        respawnRoutine = StartCoroutine(RespawnEnemyRoutine());
    }

    private void UnsubscribeCurrentEnemy()
    {
        if (currentEnemyScript != null)
        {
            currentEnemyScript.Died -= HandleEnemyDied;
            currentEnemyScript = null;
        }

        currentEnemy = null;
    }

    private IEnumerator RespawnEnemyRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnEnemy();
        respawnRoutine = null;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        const int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(0f, spawnRadius);

            Vector3 randomPosition = spawnCenter + new Vector3(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance,
                0f
            );

            if (playerObject == null)
            {
                return randomPosition;
            }

            float distanceToPlayer = Vector3.Distance(randomPosition, playerObject.transform.position);
            if (distanceToPlayer >= minDistanceFromPlayer)
            {
                return randomPosition;
            }
        }

        Debug.LogWarning("Could not find a valid enemy spawn position near the requested center.");
        return spawnCenter;
    }
}

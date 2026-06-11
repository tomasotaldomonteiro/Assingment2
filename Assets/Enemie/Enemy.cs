using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")] public float maxHealth = 30f;
    private float currentHealth;

    [Header("Damage Settings")] public float damageToPlayer = 10f;
    public float damageCooldown = 1f; 
    private float lastDamageTime = 0f;

    [Header("Respawn Settings")] public GameObject enemyPrefab; 
    public float spawnDelay = 2f; 
    private Vector3 spawnPosition;

    [Header("Movement Settings")]
    public float moveSpeed = 3f; // Speed at which enemy moves towards player
    public float detectionRange = 15f; // Range at which enemy can see the player
    
    private Rigidbody2D rb;
    private bool isDead = false;
    private Transform playerTransform;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        
        
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (isDead) return;

        // Follow the player if they're in detection range
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                FollowPlayer();
            }
            else
            {
                // Stop moving if player is out of range
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }
    

    void FollowPlayer()
    {
        if (playerTransform == null || rb == null) return;
        
        // Calculate direction to player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // Move towards player
        rb.linearVelocity = direction * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        isDead = true;
        Debug.Log("Enemy died!");
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if enough time has passed since last damage
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                DamagePlayer(other);
                lastDamageTime = Time.time;
            }
        }
    }


    void DamagePlayer(Collider2D playerCollider)
    {
        PlayerController playerController = playerCollider.GetComponent<PlayerController>();

        if (playerController != null)
        {
            Debug.Log($"Player hit! Taking {damageToPlayer} damage");
            playerController.TakeDamage(damageToPlayer);
        }
    }
}
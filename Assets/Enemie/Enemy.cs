using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public event Action Died;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 30f;
    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform spriteTransform;

    [Header("Damage Settings")]
    [SerializeField] private float damageToPlayer = 10f;
    [SerializeField] private float damageCooldown = 1f;
    private float lastDamageTime = 0f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 15f;

    private Rigidbody2D rb;
    private bool isDead = false;
    private Transform playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
            return;
        }

        playerTransform = player.transform;
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (playerTransform == null || rb == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
            return;
        }

        rb.linearVelocity = Vector2.zero;
    }

    private void FollowPlayer()
    {
        if (playerTransform == null || rb == null)
        {
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        spriteTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        if (damage <= 0f)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                DamagePlayer(other);
                lastDamageTime = Time.time;
            }
        }
    }


    private void DamagePlayer(Collider2D playerCollider)
    {
        if (playerCollider.TryGetComponent(out PlayerController playerController))
        {
            Debug.Log($"Player hit! Taking {damageToPlayer} damage");
            playerController.TakeDamage(damageToPlayer);
        }
    }
}
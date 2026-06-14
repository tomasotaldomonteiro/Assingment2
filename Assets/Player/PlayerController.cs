
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    public float slowedSpeed = 1.5f;
    public float normalSpeed = 5f;
    private float currentSpeed;
   
    //Bullet Stuff
    public GameObject bulletPrefab;
    public Transform ShootingPoint;
    public float bulletSpeed = 10f;
    
       private Rigidbody2D rb;
       private Vector2 movement;
       private Vector2 facingDirection = Vector2.right;
   
       void Awake()
       {
           // Initialize health early so UI can read the correct value in its Start
           currentHealth = maxHealth;
            // If UIManager is present, force it to show the initial health number immediately
            if (UIManager.instance != null)
            {
                UIManager.instance.SetHealthNumber(currentHealth);
            }
       }

       void Start()
       {
           rb = GetComponent<Rigidbody2D>();
           currentSpeed = normalSpeed;
       }
   
       void Update()
       {
           movement.x = Input.GetAxisRaw("Horizontal");
           movement.y = Input.GetAxisRaw("Vertical");
   
           movement = movement.normalized;
           if (movement != Vector2.zero)
           {
              facingDirection = movement;
           }
                      
                              
           if (Input.GetKeyDown(KeyCode.Space))
           { 
               Shoot();
           }
           
           UpdateShootingPoint();
           
       }
       void UpdateShootingPoint()
       {
           ShootingPoint.localPosition = facingDirection * 0.5f;
       }
   
       void FixedUpdate()
       {
           rb.linearVelocity = movement * currentSpeed;
       }
       
       void Shoot()
       {
           Vector2 dir = facingDirection.normalized;

           Vector2 spawnPos = (Vector2)transform.position + dir * 0.6f;

           GameObject bullet = Instantiate(
               bulletPrefab,
               spawnPos,
               Quaternion.identity
           );

           Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
           rb.linearVelocity = dir * bulletSpeed;
       }
       
           private void OnTriggerEnter2D(Collider2D other)
           {
               Debug.Log("Entered: " + other.name);
               if (other.CompareTag("SlowZone"))
               {
                   Debug.Log(" changing speed");
                   currentSpeed = slowedSpeed;
               }
           }

           private void OnTriggerExit2D(Collider2D other)
           {
               if (other.CompareTag("SlowZone"))
               {
                   Debug.Log(" changing speed");
                   currentSpeed = normalSpeed;
               }
           }

           public void TakeDamage(float damage)
           {
               currentHealth -= damage;
               Debug.Log($"Player took {damage} damage. Remaining health: {currentHealth}");

               // Notify the UI manager
               if (UIManager.instance != null)
               {
                   UIManager.instance.UpdateHealthDisplay();
               }

               // Check if player is dead
               if (currentHealth <= 0)
               {
                   Die();
               }
           }

         
           public float GetHealth()
           {
               return currentHealth;
           }

         
           public float GetMaxHealth()
           {
               return maxHealth;
           }

      
           void Die()
           {
               Debug.Log("Player died!");
                      gameObject.SetActive(false);

                      // Show death UI
                      if (UIManager.instance != null)
                      {
                          UIManager.instance.ShowDeathScreen();
                      }
           }
}

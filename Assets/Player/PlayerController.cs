
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
           
           if (movement != Vector2.zero)
           {
               facingDirection = movement;

               if (facingDirection.x > 0)
                   ShootingPoint.localPosition = new Vector3(0.5f, 0f, 0f);
               else if (facingDirection.x < 0)
                   ShootingPoint.localPosition = new Vector3(-0.5f, 0f, 0f);
               else if (facingDirection.y > 0)
                   ShootingPoint.localPosition = new Vector3(0f, 0.5f, 0f);
               else if (facingDirection.y < 0)
                   ShootingPoint.localPosition = new Vector3(0f, -0.5f, 0f);
           }
       }
   
       void FixedUpdate()
       {
           rb.velocity = movement * currentSpeed;
       }
       
       void Shoot()
           {
               GameObject bullet = Instantiate(
                   bulletPrefab,
                   ShootingPoint.position,
                   Quaternion.identity
               );
       
               Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
               bulletRb.velocity = facingDirection * bulletSpeed;
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
}

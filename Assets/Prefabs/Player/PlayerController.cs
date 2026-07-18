using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] private float slowedSpeed = 1.5f;
    [SerializeField] private float normalSpeed = 5f;
    private float currentSpeed;
    [FormerlySerializedAs("Sprite")]
    [SerializeField] private Transform spriteTransform;

    [Header("Bullet Stuff")]
    [SerializeField] private GameObject bulletPrefab;
    [FormerlySerializedAs("ShootingPoint")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Phone / Mobile Input")]
    [Tooltip("Enable on-screen phone controls / touch input")]
    [SerializeField] private bool enablePhoneInput = true;
    [Tooltip("How far a touch must move (pixels) to count as full movement")]
    [SerializeField] private float touchMoveDistance = 120f;

    public event Action<float, float> HealthChanged;
    public event Action Died;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 facingDirection = Vector2.right;
    private Vector2 _touchMovement;
    private int _movementTouchId = -1;
    private Vector2 _movementTouchStart;
    private bool _moveUpPressed;
    private bool _moveDownPressed;
    private bool _moveLeftPressed;
    private bool _moveRightPressed;
    private bool _shootRequested;

    private void Awake()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = normalSpeed;
        UpdateShootingPoint();
    }

    private void Update()
    {
        movement = ReadMovementInput();
        if (movement != Vector2.zero)
        {
            facingDirection = movement;
        }

        if (spriteTransform != null && facingDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
            spriteTransform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        bool shootInput = Input.GetMouseButtonDown(0) || ConsumeShootRequest();
        if (shootInput)
        {
            Shoot();
        }

        UpdateShootingPoint();
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = movement * currentSpeed;
    }

    public void SetMoveUp(bool pressed)
    {
        _moveUpPressed = pressed;
    }

    public void SetMoveDown(bool pressed)
    {
        _moveDownPressed = pressed;
    }

    public void SetMoveLeft(bool pressed)
    {
        _moveLeftPressed = pressed;
    }

    public void SetMoveRight(bool pressed)
    {
        _moveRightPressed = pressed;
    }

    public void RequestShoot()
    {
        _shootRequested = true;
    }

    private Vector2 ReadMovementInput()
    {
        Vector2 buttonMovement = new Vector2(
            (_moveRightPressed ? 1f : 0f) - (_moveLeftPressed ? 1f : 0f),
            (_moveUpPressed ? 1f : 0f) - (_moveDownPressed ? 1f : 0f)
        );

        if (buttonMovement != Vector2.zero)
        {
            return buttonMovement.normalized;
        }

        if (enablePhoneInput && Input.touchSupported)
        {
            ReadTouchInput();

            if (_movementTouchId != -1)
            {
                return _touchMovement;
            }
        }

        Vector2 keyboardMovement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        return keyboardMovement.normalized;
    }

    private void ReadTouchInput()
    {
        _touchMovement = Vector2.zero;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (_movementTouchId == -1 && touch.phase == TouchPhase.Began && touch.position.x <= Screen.width * 0.5f)
            {
                _movementTouchId = touch.fingerId;
                _movementTouchStart = touch.position;
            }

            if (touch.fingerId == _movementTouchId)
            {
                Vector2 delta = touch.position - _movementTouchStart;
                _touchMovement = Vector2.ClampMagnitude(delta / Mathf.Max(1f, touchMoveDistance), 1f);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _movementTouchId = -1;
                    _touchMovement = Vector2.zero;
                }

                continue;
            }

            if (touch.phase == TouchPhase.Began && touch.position.x > Screen.width * 0.5f)
            {
                _shootRequested = true;
            }
        }
    }

    private bool ConsumeShootRequest()
    {
        if (!_shootRequested)
        {
            return false;
        }

        _shootRequested = false;
        return true;
    }

    private void OnGUI()
    {
        if (!enablePhoneInput || !Input.touchSupported)
        {
            return;
        }

        float buttonSize = Mathf.Clamp(Mathf.Min(Screen.width, Screen.height) * 0.18f, 120f, 220f);
        float margin = Mathf.Clamp(buttonSize * 0.2f, 20f, 40f);
        Rect shootRect = new Rect(
            Screen.width - buttonSize - margin,
            Screen.height - buttonSize - margin,
            buttonSize,
            buttonSize
        );

        Color previousColor = GUI.color;
        GUI.color = new Color(1f, 0.45f, 0.2f, 0.85f);

        if (GUI.Button(shootRect, "SHOOT"))
        {
            RequestShoot();
        }

        GUI.color = previousColor;
    }

    private void UpdateShootingPoint()
    {
        if (shootingPoint != null)
        {
            shootingPoint.localPosition = facingDirection.normalized * 0.5f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || shootingPoint == null)
        {
            return;
        }

        Vector2 dir = facingDirection.normalized;
        Vector2 spawnPos = (Vector2)transform.position + dir * 0.6f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        if (bullet.TryGetComponent(out Rigidbody2D bulletRb))
        {
            bulletRb.linearVelocity = dir * bulletSpeed;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SlowZone"))
        {
            currentSpeed = slowedSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SlowZone"))
        {
            currentSpeed = normalSpeed;
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
        {
            return;
        }

        currentHealth -= damage;
        NotifyHealthChanged();

        if (currentHealth <= 0f)
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

    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Died?.Invoke();
        gameObject.SetActive(false);
    }
}

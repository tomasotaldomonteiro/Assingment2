using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class KeyPickUp : MonoBehaviour
{
    [SerializeField] private KeyScriptableObject key;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (key == null)
        {
            Debug.LogWarning($"{name} is missing a key asset.");
            return;
        }

        spriteRenderer.sprite = key.KeySprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (key == null)
        {
            return;
        }

        if (!other.TryGetComponent(out PlayerInventory inventory))
        {
            return;
        }

        inventory.AddKey(key);
        Destroy(gameObject);
    }
}

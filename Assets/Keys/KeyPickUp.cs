using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class KeyPickUp : MonoBehaviour
{
    public KeyScriptableObject key;
    
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = key.keySprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddKey(key);
            Destroy(gameObject);
        }
    }
}

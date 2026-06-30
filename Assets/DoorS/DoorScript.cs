using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public KeyScriptableObject[] requiredKeys;

    public Vector3 openPosition;
    public Vector3 openRotation;

    private bool opened;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (opened)
            return;

        PlayerInventory inventory = collision.collider.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        foreach (KeyScriptableObject key in requiredKeys)
        {
            if (!inventory.HasKey(key))
                return;
        }

        OpenDoor();
    }

    void OpenDoor()
    {
        opened = true;

        transform.position = openPosition;
        transform.rotation = Quaternion.Euler(openRotation);
    }
}

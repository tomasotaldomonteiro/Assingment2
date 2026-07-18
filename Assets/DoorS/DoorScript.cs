using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private KeyScriptableObject[] requiredKeys;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 openRotation;

    private bool opened;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (opened)
        {
            return;
        }

        if (!collision.collider.TryGetComponent(out PlayerInventory inventory))
        {
            return;
        }

        if (requiredKeys == null || requiredKeys.Length == 0)
        {
            OpenDoor();
            return;
        }

        foreach (KeyScriptableObject key in requiredKeys)
        {
            if (!inventory.HasKey(key))
                return;
        }

        OpenDoor();
    }

    private void OpenDoor()
    {
        opened = true;
        transform.SetPositionAndRotation(openPosition, Quaternion.Euler(openRotation));
    }
}

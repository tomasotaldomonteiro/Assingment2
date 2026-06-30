using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private HashSet<KeyScriptableObject> keys = new HashSet<KeyScriptableObject>();

    public void AddKey(KeyScriptableObject key)
    {
        keys.Add(key);
        Debug.Log("Picked up " + key.keyName);
    }

    public bool HasKey(KeyScriptableObject key)
    {
        return keys.Contains(key);
    }
}

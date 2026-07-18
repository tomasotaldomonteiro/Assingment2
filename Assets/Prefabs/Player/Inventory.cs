using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly HashSet<KeyScriptableObject> keys = new HashSet<KeyScriptableObject>();

    public event Action<KeyScriptableObject> KeyAdded;

    public void AddKey(KeyScriptableObject key)
    {
        if (key == null)
        {
            Debug.LogWarning("Cannot add a null key to the inventory.");
            return;
        }

        if (keys.Add(key))
        {
            Debug.Log("Picked up " + key.KeyName);
            KeyAdded?.Invoke(key);
        }
    }

    public bool HasKey(KeyScriptableObject key)
    {
        return key != null && keys.Contains(key);
    }
}

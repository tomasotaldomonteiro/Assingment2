using UnityEngine;

[CreateAssetMenu(fileName = "KeyScriptableObject", menuName = "Scriptable Objects/KeyScriptableObject")]
public class KeyScriptableObject : ScriptableObject
{
    [SerializeField] private string keyName;
    [SerializeField] private Sprite keySprite;

    public string KeyName => keyName;
    public Sprite KeySprite => keySprite;
}

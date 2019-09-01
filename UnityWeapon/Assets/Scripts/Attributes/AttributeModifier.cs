using UnityEngine;

[CreateAssetMenu(menuName = "AttributeModifier/Create")]
public class AttributeModifier : ScriptableObject
{
    public enum ValueType { Relative, Absolute }

    public StringReference Name;
    public ValueType Type;
    [Range(0, ushort.MaxValue)] public float Value = 1.0f;
}

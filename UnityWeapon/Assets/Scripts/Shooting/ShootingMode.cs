using UnityEngine;
using Attribute;

[CreateAssetMenu(menuName = "Computing/Shooting/Mode")]
public abstract class ShootingMode : ScriptableObject
{
    [SerializeField] protected ushort _bulletsToPerformShot = 1;
    [SerializeField] protected StaticAttributes _attributes;

    public abstract bool IsExecutable(Weapon weapon);
    public abstract void Perform(Weapon weapon);
}

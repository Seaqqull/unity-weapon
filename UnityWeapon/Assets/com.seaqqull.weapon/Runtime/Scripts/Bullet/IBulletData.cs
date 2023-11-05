using UnityEngine;

namespace Weapons.Bullets
{
    public interface IBulletData
    {
        LayerMask TargetMask { get; }
        bool LookRotation { get; }
        int Damage { get; }
        float Speed { get; }
        float Range { get; }
    }
}
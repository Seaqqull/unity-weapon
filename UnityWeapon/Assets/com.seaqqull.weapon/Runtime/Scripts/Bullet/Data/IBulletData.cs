using UnityEngine;


namespace Weapons.Bullets
{
    public interface IBulletData
    {
        static IBulletData Empty { get; } = new BulletData();

        LayerMask TargetMask { get; }
        bool LookRotation { get; }
        int Damage { get; }
        float Speed { get; }
        float Range { get; }
    }
}
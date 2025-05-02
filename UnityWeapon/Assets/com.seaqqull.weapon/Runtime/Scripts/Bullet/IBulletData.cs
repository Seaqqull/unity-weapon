using Weapons.Aiming.Following;
using UnityEngine;


namespace Weapons.Bullets
{
    public interface IBulletData
    {
        FollowerFabricSO Follower { get; }
        LayerMask TargetMask { get; }
        bool LookRotation { get; }
        int Damage { get; }
        float Speed { get; }
        float Range { get; }
    }
}
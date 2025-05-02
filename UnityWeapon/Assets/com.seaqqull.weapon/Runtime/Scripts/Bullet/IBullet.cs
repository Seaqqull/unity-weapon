using UnityEngine;
using Weapons.Aiming.Following;


namespace Weapons.Bullets
{
    public interface IBullet
    {
        void Launch();
        void Bake(IBulletData data);
        void BakeFlowDirection(Transform bulletFlow);
        void BakeFlowDirection(IFollower follower);
        void BakeFlowDirection(Transform bulletFlow, Quaternion rotation);
    }
}
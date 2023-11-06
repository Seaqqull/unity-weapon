using UnityEngine;
using Weapons.Aiming;

namespace Weapons.Bullets.Data
{
    public interface IBullet
    {
        void Launch();
        void Bake(IBulletData data);
        void BakeFlowDirection(Transform bulletFlow);
        void BakeFlowDirection(Line[] bulletFlow);
        void BakeFlowDirection(Transform bulletFlow, Quaternion rotation);
    }
}
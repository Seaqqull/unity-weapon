using UnityEngine;
using Weapons.Aiming;

namespace Weapons.Bullets
{
    public interface IBullet
    {
        void Launch();
        void Bake(IBulletData data);
        void BakeFlowDirection(Transform bulletFlow);
        void BakeFlowDirection(Line[] flow);
        void BakeFlowDirection(Transform bulletFlow, Quaternion rotation);
    }
}
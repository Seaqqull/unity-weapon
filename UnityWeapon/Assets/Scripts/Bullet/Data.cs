using UnityEngine;
using Weapons.Aiming;

namespace Weapons.Bullets.Data
{
    public interface IBullet
    {
        void Launch();
        void Bake(BulletDataSO data);
        public void BakeFlowDirection(Transform bulletFlow);
        public void BakeFlowDirection(Accuracy.Line bulletFlow);
        public void BakeFlowDirection(Transform bulletFlow, Quaternion rotation);
    }
}
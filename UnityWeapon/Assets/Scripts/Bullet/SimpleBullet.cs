using UnityEngine;


namespace Weapons.Bullets
{
    public class SimpleBullet : ImpactBullet
    {
        protected override void OnTriggerEnter(Collider other)
        {
            var affectedEntity = CheckBulletCollision(other);
            if (affectedEntity != null)
            {
                OnBulletHit();
                OnTargetHit(affectedEntity);
            }                        

            OnBulletDestroy();
        }

    }
}

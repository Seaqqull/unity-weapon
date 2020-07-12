using UnityEngine;


namespace Weapons.Bullets
{
    public class SimpleBullet : ImpactBullet
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if ((_isLaunched) &&
                ((1 << other.gameObject.layer) & _targetMask) != 0)
            {
                Utility.Data.IEntity affectedTarget = other.GetComponent<Utility.Data.IEntity>();

                if (affectedTarget != null)
                {
                    OnBulletHit();
                    OnTargetHit(affectedTarget);
                }
            }

            OnBulletDestroy();
        }

    }
}

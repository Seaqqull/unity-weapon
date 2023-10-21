using UnityEngine;


namespace Weapons.Bullets
{
    public abstract class ImpactBullet : ActiveBullet
    {
        [SerializeField] private float _impactPower;

        public float ImpactPower => _impactPower;


        protected override void OnTargetHit(global::Weapon.Utility.IEntity affectedEntity)
        {
            base.OnTargetHit(affectedEntity);

            affectedEntity.ApplyForce(Transform.forward * _impactPower);
        }

    }
}

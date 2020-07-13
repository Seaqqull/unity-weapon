using UnityEngine;


namespace Weapons.Bullets
{
    public abstract class ImpactBullet : ActiveBullet
    {
        [SerializeField] private float _impactPower;

        public float ImpactPower
        {
            get { return this._impactPower; }
        }


        protected override void OnTargetHit(Utility.IEntity affectedEntity)
        {
            base.OnTargetHit(affectedEntity);

            affectedEntity.ApplyForce(Transform.forward * _impactPower);
        }

    }
}

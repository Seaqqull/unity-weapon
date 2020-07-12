using UnityEngine;


namespace Weapons.Bullets
{
    public abstract class RandomImpactBullet : ImpactBullet
    {
        [SerializeField, Range(0.0f, 1.0f)] protected float _impactChance = 1.0f;

        public float ImpactChance
        {
            get { return this._impactChance; }
        }


        protected override void OnTargetHit(Utility.Data.IEntity affectedEntity)
        {
            base.OnTargetHit(affectedEntity);

            if (Random.Range(0.0f, 1.0f) <= _impactChance)
                affectedEntity.ApplyForce(Transform.forward * ImpactPower);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Bullets
{
    public class DelayedBullet : RandomImpactBullet
    {
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_lifetime = 0.5f;

        private List<Utility.Data.IEntity> _affectedEntities;


        protected override void OnTriggerEnter(Collider other)
        {
            if ((!_isLaunched) ||
                (((1 << other.gameObject.layer) & _targetMask) == 0)) return;

            Utility.Data.IEntity affectedEntity = other.GetComponent<Utility.Data.IEntity>();
            if ((affectedEntity != null) && !IsEntityAffected(affectedEntity))
            {
                OnBulletHit();
                OnTargetHit(affectedEntity);
                
                _affectedEntities.Add(affectedEntity);
            }
        }


        protected override void OnBulletStart()
        {
            base.OnBulletStart();

            _affectedEntities = new List<Utility.Data.IEntity>();
            Invoke("OnBulletDestroy", m_lifetime);
        }

        private bool IsEntityAffected(Utility.Data.IEntity hittedEntity)
        {
            for (int i = 0; i < _affectedEntities.Count; i++)
            {
                if (_affectedEntities[i].Id == hittedEntity.Id)
                    return true;
            }
            return false;
        }

    }
}

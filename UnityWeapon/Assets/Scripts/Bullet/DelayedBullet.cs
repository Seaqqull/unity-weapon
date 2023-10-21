using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;


namespace Weapons.Bullets
{
    public class DelayedBullet : RandomImpactBullet, global::Weapon.Utility.IRunLater
    {
        [SerializeField] [Range(0, ushort.MaxValue)] private float _lifetime = 0.5f;

        private List<global::Weapon.Utility.IEntity> _affectedEntities;


        protected override void OnTriggerEnter(Collider other)
        {
            global::Weapon.Utility.IEntity affectedEntity = CheckBulletCollision(other);
            if ((affectedEntity == null) || IsEntityAffected(affectedEntity))
                return;

            OnBulletHit();
            OnTargetHit(affectedEntity);

            _affectedEntities.Add(affectedEntity);
        }


        protected override void OnBulletStart()
        {
            base.OnBulletStart();

            _affectedEntities = new List<global::Weapon.Utility.IEntity>();

            RunLater(()=> { OnBulletDestroy(); }, _lifetime);
        }

        private bool IsEntityAffected(global::Weapon.Utility.IEntity hittedEntity)
        {
            for (var i = 0; i < _affectedEntities.Count; i++)
            {
                if (_affectedEntities[i].Id == hittedEntity.Id)
                    return true;
            }
            return false;
        }


        #region RunLater
        public void RunLater(Action method, float waitSeconds)
        {
            RunLaterValued(method, waitSeconds);
        }

        public Coroutine RunLaterValued(Action method, float waitSeconds)
        {
            if ((waitSeconds < 0) || (method == null))
                return null;

            return StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        public IEnumerator RunLaterCoroutine(Action method, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            method();
        }
        #endregion
    }
}

using UnityEngine;
using System;


namespace Weapons.Bullets
{
    public class SimpleBulletRay : ImpactBullet
    {
        public float ProjectileSpeedMultiplier;

        private Vector3 _previousPosition;


        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Math.Abs(ProjectileSpeedMultiplier) > float.Epsilon)
            {
                _speed *= ProjectileSpeedMultiplier;
                _rigidbody.AddForce(Transform.forward * (_speed * Time.fixedDeltaTime));
            }

            CheckCollision(_previousPosition);

            _previousPosition = Transform.position;
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            Utility.IEntity affectedEntity = CheckBulletCollision(other);

            if (affectedEntity != null)
            {
                OnBulletHit();
                OnTargetHit(affectedEntity);
            }

            OnBulletDestroy(other, true);
        }


        protected override void InitStart()
        {
            base.InitStart();

            _previousPosition = Transform.position;
        }

        private void CheckCollision(Vector3 position)
        {
            RaycastHit hit;

            float dist = Vector3.Distance(Transform.position, position);
            Vector3 direction = Transform.position - position;
            Ray ray = new Ray(position, direction);            

            if (!Physics.Raycast(ray, out hit, dist, _targetMask))
                return;

            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            Vector3 pos = hit.point;

            Utility.IEntity affectedEntity = CheckBulletCollision(hit.collider);

            if (affectedEntity != null) 
            { 
                    OnBulletHit();
                    OnTargetHit(affectedEntity);
            }

            OnBulletDestroy(hit, true);            
        }
    }
}

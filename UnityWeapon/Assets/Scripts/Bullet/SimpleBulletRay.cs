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

        protected override void OnTriggerEnter(Collider other) { }


        protected override void InitStart()
        {
            base.InitStart();

            _previousPosition = Transform.position;
        }

        private void CheckCollision(Vector3 position)
        {
            RaycastHit hit;
            Vector3 direction = Transform.position - position;
            Ray ray = new Ray(position, direction);
            float dist = Vector3.Distance(Transform.position, position); //!

            if (Physics.Raycast(ray, out hit, dist, _targetMask))
            {
                //Transform.position = hit.point;
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                Vector3 pos = hit.point;


                if ((_isLaunched) &&
                    ((1 << hit.collider.gameObject.layer) & _targetMask) != 0)
                {
                    Utility.IEntity affectedTarget = hit.collider.GetComponent<Utility.IEntity>();

                    if (affectedTarget != null)
                    {
                        OnBulletHit();
                        OnTargetHit(affectedTarget);
                    }
                }

                OnBulletDestroy(hit, true);
            }
        }
    }
}

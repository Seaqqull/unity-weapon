using UnityEngine;


namespace Weapons.Bullets
{
    public class SimpleBulletRay : ImpactBullet
    {
        /// <summary>
        /// Rewrite using some sort of modificators.
        /// </summary>
        public float ProjectileSpeedMultiplier;

        private Vector3 _previousPosition;


        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Mathf.Abs(ProjectileSpeedMultiplier) > float.Epsilon)
            {
                _speed *= ProjectileSpeedMultiplier;
                _rigidbody.AddForce(Transform.forward * (_speed * Time.fixedDeltaTime));
            }

            var currentPosition = Transform.position;
            CheckCollision(_previousPosition, currentPosition);
            _previousPosition = currentPosition;
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            var affectedEntity = CheckBulletCollision(other);
            if (affectedEntity != null)
            {
                OnBulletHit();
                OnTargetHit(affectedEntity);
            }

            OnBulletDestroy(other, true);
        }

        protected override void OnBulletStart()
        {
            _previousPosition = Transform.position;

            base.OnBulletStart();
        }

        private void CheckCollision(Vector3 previousPosition, Vector3 currentPosition)
        {
            var dist = Vector3.Distance(currentPosition, previousPosition);
            var direction = currentPosition - previousPosition;
            var ray = new Ray(previousPosition, direction);            

            if (!Physics.Raycast(ray, out var hit, dist, _targetMask))
                return;

            var rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            var pos = hit.point;

            var affectedEntity = CheckBulletCollision(hit.collider);
            if (affectedEntity != null) 
            { 
                OnBulletHit();
                OnTargetHit(affectedEntity);
            }

            OnBulletDestroy(hit, true);            
        }
    }
}

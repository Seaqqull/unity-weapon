using UnityEngine;


namespace Weapons.Bullets
{
    public class SimpleBulletRay : ImpactBullet
    {
        /// <summary>
        /// Rewrite using some sort of modificators.
        /// </summary>
        [field: SerializeField] public float ProjectileSpeedMultiplier { get; private set; }

        private Vector3 _previousPosition;


        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Mathf.Abs(ProjectileSpeedMultiplier) > float.Epsilon)
            {
                Speed += (ProjectileSpeedMultiplier / (1 / Time.fixedDeltaTime));
                Speed = Mathf.Max(Speed, 0.0f);
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

            if (!Physics.Raycast(ray, out var hit, dist, TargetMask))
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

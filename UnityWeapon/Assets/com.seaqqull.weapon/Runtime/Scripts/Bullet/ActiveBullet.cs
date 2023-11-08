using Weapons.Aiming.Following;
using Weapon.Storage.Data;
using Weapons.Aiming;
using UnityEngine;
using Weapon.Base;
using System;


namespace Weapons.Bullets
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ActiveBullet : BaseMonoBehaviour, IBullet, IBulletData, IPoolable
    {
        protected Rigidbody _rigidbody;
        protected Vector3 _startPosition;
        protected float _squaredRange;
        protected bool _isLaunched;

        protected IFollower _flowFollower;

        public event Action<ActiveBullet> OnDestroy;
        public event Action<ActiveBullet> OnLaunch;
        public event Action<ActiveBullet> OnHit;

        public FollowType FollowType { get; set; }
        public LayerMask TargetMask { get; set; }
        public bool LookRotation { get; set; }
        public float Speed { get; set; }
        public float Range { get; set; }
        public int Damage { get; set; }

        public float PassedDistance => (Position - _startPosition).sqrMagnitude;
        public GameObject GameObject => GameObj;
        public IPool Pooler { get; set; }


        protected override void Awake()
        {
            base.Awake();

            TryGetComponent<Rigidbody>(out _rigidbody);
        }

        protected virtual void FixedUpdate()
        {
            if (!_isLaunched || Speed == 0.0f) return;

            var passedDistance = PassedDistance;
            if (Range > 0.0f && passedDistance > _squaredRange)
                OnBulletDestroy();
            if (_flowFollower.IsValid())
                _flowFollower.UpdateDirection(passedDistance);

            _rigidbody.MovePosition(Transform.position + (_flowFollower.FollowDirection * (Speed * Time.fixedDeltaTime)));
            if (LookRotation && _rigidbody.velocity != Vector3.zero)
                Transform.rotation = Quaternion.LookRotation(_flowFollower!.FollowDirection);
        }


        protected abstract void OnTriggerEnter(Collider other);


        protected virtual void OnBulletHit()
        {
            OnHit?.Invoke(this);
        }

        protected virtual void OnBulletStart()
        {
            _startPosition = Transform.position;
            
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(Transform.forward * Speed);

            OnLaunch?.Invoke(this);
        }

        protected virtual void OnBulletDestroy()
        {
            _rigidbody.velocity = Vector3.zero;
            Pooler.Return(this);
        }

        protected virtual void OnTargetHit(global::Weapon.Utility.IEntity affectedEntity)
        {
            affectedEntity.ModifyHealth(Damage);
        }

        protected virtual global::Weapon.Utility.IEntity CheckBulletCollision(Collider obstacle)
        {
            return (_isLaunched && ((1 << obstacle.gameObject.layer) & TargetMask) != 0) 
                ? obstacle.GetComponent<global::Weapon.Utility.IEntity>() 
                : null;
        }

        protected virtual void OnBulletDestroy(Collider hit, bool spawnInHit = false)
        {
            PoolIn();
        }

        protected virtual void OnBulletDestroy(RaycastHit hit, bool spawnInHit = false)
        {
            PoolIn();
        }

        protected void SpawnOnPosition(GameObject objectToSpawn, Transform hit, bool spawnInHit = false)
        {
            var rot = Quaternion.FromToRotation(Vector3.forward, hit.transform.position.normalized);
            var pos = hit.transform.position;

            var spawnedObj = Instantiate(objectToSpawn, pos, rot);
            if (spawnInHit)
                spawnedObj.transform.SetParent(hit);
        }

        protected void SpawnOnPosition(GameObject objectToSpawn, RaycastHit hit, bool spawnInHit = false)
        {
            var rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            var pos = hit.point;

            var spawnedObj = Instantiate(objectToSpawn, pos, rot);
            if (spawnInHit)
                spawnedObj.transform.SetParent(hit.collider.transform);
        }


        public void PoolIn()
        {
            OnDestroy?.Invoke(this);

            OnBulletDestroy();
        }

        public void PoolOut() { }

        public void Launch()
        {
            _isLaunched = true;

            OnBulletStart();
        }

        public void Bake(IBulletData data)
        {
            LookRotation = data.LookRotation;
            FollowType = data.FollowType;
            Damage = data.Damage;
            Speed = data.Speed;
            Range = data.Range;
            _squaredRange = Range * Range;

            TargetMask = data.TargetMask;
        }

        public void BakeFlowDirection(Line[] flow)
        {
            _flowFollower = FollowType switch
            {
                FollowType.Start => new StartFollower(flow),
                FollowType.End => new EndFollower(flow),
                FollowType.Average => new AverageFollower(flow),
                FollowType.Follow => new FlowFollower(flow),
                FollowType.SmoothedFollow => new SmoothedFlowFollower(flow),
                _ => new StartFollower(flow)
            };
            Transform.position = flow[0].From;
        }
        
        public void BakeFlowDirection(Transform bulletFlow)
        {
            BakeFlowDirection(bulletFlow, bulletFlow.rotation);
        }

        public void BakeFlowDirection(Transform bulletFlow, Quaternion rotation)
        {
            Transform.position = bulletFlow.position;
            Transform.rotation = rotation;
        }
    }
}

using Weapons.Aiming;
using UnityEngine;
using System;
using Weapon.Base;
using Weapon.Storage.Data;
using Weapons.Bullets.Data;


namespace Weapons.Bullets
{
    [Serializable]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ActiveBullet : BaseMonoBehaviour, IBullet, IPoolable
    {
        protected LayerMask _targetMask;
        protected Rigidbody _rigidbody;

        protected Vector3 _startPosition;
        protected float _squaredRange;
        protected bool _lookRotation;
        protected bool _isLaunched;
        protected float _speed;
        protected float _range;
        protected int _damage;

        public event Action<ActiveBullet> OnDestroy;
        public event Action<ActiveBullet> OnLaunch;
        public event Action<ActiveBullet> OnHit;

        public bool DistancePassed => _range > 0.0f && (Position - _startPosition).sqrMagnitude > _squaredRange;
        public GameObject GameObject => GameObj;
        public IPool Pooler { get; set; }


        protected override void Awake()
        {
            base.Awake();

            TryGetComponent<Rigidbody>(out _rigidbody);
        }

        protected virtual void FixedUpdate()
        {
            if (!_isLaunched || _speed == 0.0f) return;

            if (DistancePassed)
                OnBulletDestroy();
            if (_lookRotation && _rigidbody.velocity != Vector3.zero)
                Transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
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
            _rigidbody.AddForce(Transform.forward * _speed);

            OnLaunch?.Invoke(this);
        }

        protected virtual void OnBulletDestroy()
        {
            _rigidbody.velocity = Vector3.zero;
            Pooler.Return(this);
        }

        protected virtual void OnTargetHit(global::Weapon.Utility.IEntity affectedEntity)
        {
            affectedEntity.ModifyHealth(_damage);
        }

        protected virtual global::Weapon.Utility.IEntity CheckBulletCollision(Collider obstacle)
        {
            return (_isLaunched && ((1 << obstacle.gameObject.layer) & _targetMask) != 0) 
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
            _lookRotation = data.LookRotation;
            _damage = data.Damage;
            _speed = data.Speed;
            _range = data.Range;
            _squaredRange = _range * _range;

            _targetMask = data.TargetMask;
        }

        public void BakeFlowDirection(Transform bulletFlow)
        {
            BakeFlowDirection(bulletFlow, bulletFlow.rotation);
        }

        public void BakeFlowDirection(Line[] bulletFlow)
        {// TODO: Handle array
            Transform.position = bulletFlow[0].From;
            Transform.rotation = Quaternion.LookRotation(bulletFlow[0].Direction);
        }
        
        public void BakeFlowDirection(Transform bulletFlow, Quaternion rotation)
        {
            Transform.position = bulletFlow.position;
            Transform.rotation = rotation;
        }

    }
}

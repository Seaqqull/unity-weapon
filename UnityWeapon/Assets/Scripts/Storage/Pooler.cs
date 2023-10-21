using System.Collections.Generic;
using UnityEngine;
using Weapon.Storage.Data;

namespace Weapon.Storage
{
    public class Pooler : IPool
    {
        private GameObject _spawnObject;
        protected GameObject _spawned;
        protected GameObject _queue;
        private GameObject _owner;

        protected Queue<GameObject> _objectsToPool;

        public StorageData Properties { get; private set; }


        public Pooler(GameObject owner, GameObject spawnObject, StorageData properties)
        {
            _spawnObject = spawnObject;
            _owner = owner;

            Properties = properties;
            
            _spawned = new GameObject($"Spawned - {properties.Id}");
            _queue = new GameObject($"Queue - {properties.Id}");

            _spawned.transform.parent = _owner.transform;
            _queue.transform.parent = _owner.transform;

            _objectsToPool = new Queue<GameObject>();

            PoolExtend(Properties.PoolAmount);
        }
        
        /// <summary> 
        /// Instantiates object and adds it too pool
        /// </summary>
        protected virtual void PoolCreate()
        {
            var dummyIn = GameObject.Instantiate(_spawnObject, _queue.transform);
            dummyIn.SetActive(false);
            
            if (dummyIn.TryGetComponent<IPoolable>(out var poolableDummy))
                poolableDummy.Pooler = this;

            _objectsToPool.Enqueue(dummyIn);
        }

        /// <summary>
        /// Instantiates [amount] of given objects and inserts into the queue
        /// </summary>
        /// <param name="amount">Amount of items to be instantiated and inserted into the queue</param>
        protected void PoolExtend(int amount)
        {
            for (var i = 0; i < amount; i++)
                PoolCreate();
        }

        /// <summary>
        /// Pools object.
        /// </summary>
        protected virtual GameObject PoolOut()
        {
            var dummyOut = _objectsToPool.Dequeue();

            var dummyTransform = dummyOut.transform;
            dummyTransform.parent = _spawned.transform;

            dummyOut.SetActive(true);

            return dummyOut;
        }

        /// <summary>
        /// Adds object to pool.
        /// </summary>
        /// <param name="dummyIn">Object to be added</param>
        protected virtual void PoolIn(GameObject dummyIn)
        {
            dummyIn.SetActive(false);

            var dummyTransform = dummyIn.transform;
            dummyTransform.parent = _queue.transform;
            dummyTransform.position = Vector3.zero;

            _objectsToPool.Enqueue(dummyIn);
        }


        public GameObject Pool()
        {
            if (_spawnObject == null) return null;

            if (_objectsToPool.Count == 0)
                PoolExtend(Properties.ExpansionAmount);

            var dummyOut = PoolOut();
            if (dummyOut.TryGetComponent<IPoolable>(out var poolableDummy))
                poolableDummy.PoolOut();

            return dummyOut;
        }

        public void Return(IPoolable poolable)
        {
            var reduceAmount = Properties.ReductionAmount - 1;
            if (_objectsToPool.Count < (Properties.PoolAmount + reduceAmount))
            {
                PoolIn(poolable.GameObject);
                return;
            }
            
            for (var i = 0; i < reduceAmount; i++)
                GameObject.Destroy(_objectsToPool.Dequeue());
            GameObject.Destroy(poolable.GameObject);
        }
    }
}
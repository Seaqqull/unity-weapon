using System.Collections.Generic;
using Weapon.Storage.Data;
using UnityEngine;

namespace Weapon.Storage
{
    public class Storage : IStorage
    {
        private GameObject _owner;
        private Dictionary<int, Pooler> _pools = new();


        public Storage(GameObject owner)
        {
            _owner = owner;
        }


        public void Populate(GameObject spawnObject, StorageData properties)
        {
            if (!_pools.ContainsKey(properties.Id))
                _pools.Add(properties.Id, new Pooler(_owner, spawnObject, properties));
        }

        public GameObject Pool(int id)
        {
            return _pools.TryGetValue(id, out var pool) ? pool.Pool() : null;
        }
    }
}
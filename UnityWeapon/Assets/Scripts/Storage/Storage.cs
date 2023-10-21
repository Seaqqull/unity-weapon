using System.Collections.Generic;
using Weapon.Storage.Data;
using UnityEngine;

namespace Weapon.Storage
{
    public class Storage : IStorage
    {
        private GameObject _owenr;
        private Dictionary<int, Pooler> _pools = new();


        public Storage(GameObject owner)
        {
            _owenr = owner;
        }


        public void Populate(GameObject spawnObject, StorageData properties)
        {
            if (!_pools.TryGetValue(properties.Id, out var pool))
                _pools.Add(properties.Id, new Pooler(_owenr, spawnObject, properties));
        }

        public GameObject Pool(int id)
        {
            return _pools.TryGetValue(id, out var pool) ? pool.Pool() : null;
        }
    }
}
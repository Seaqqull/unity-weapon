using UnityEngine;


namespace Weapon.Storage.Data
{
    public class StorageData
    {
        public readonly int Id;
        public readonly int PoolAmount;
        public readonly int ExpansionAmount;
        public readonly int ReductionAmount;

        public StorageData(int id, int poolAmount, int expansionAmount, int reductionAmount)
        {
            Id = id;
            PoolAmount = poolAmount;
            ExpansionAmount = expansionAmount;
            ReductionAmount = reductionAmount;
        }
    }
    
    public interface IPoolable
    {
        IPool Pooler { get; set; }
        GameObject GameObject { get; }

        void PoolIn();
        void PoolOut();
    }
    
    public interface IPool
    {
        void Return(IPoolable poolable);
    }
    
    public interface IStorage
    {
        GameObject Pool(int id);
        void Populate(GameObject spawnObject, StorageData properties); 
    }
}
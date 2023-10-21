using System.Collections.Generic;
using UnityEngine;

namespace Weapon.Utility
{
    public class ComputingHandler : ScriptableObject { }

    public class ComputingData<T> : ScriptableObject 
        where T : struct
    {
        [SerializeField] public T Data;
    }

    public class ComputingObject<THandler, TObject>
        where THandler : ComputingHandler
    {
#pragma warning disable 0649
        [SerializeField] private THandler _handler;
        [SerializeField] private TObject[] _objects;

        private IReadOnlyList<TObject> _objectsRestricted;
#pragma warning restore 0649

        public IReadOnlyList<TObject> Objects => _objectsRestricted ??= _objects;
        public THandler Handler => _handler;
    }

    public class ComputingData<THandler, TData>
    where THandler : ComputingHandler
    {
#pragma warning disable 0649
        [SerializeField] private THandler _handler;
        [SerializeField] private TData[] _data;

        private IReadOnlyList<TData> _dataRestricted;
#pragma warning restore 0649

        public IReadOnlyList<TData> Data => _dataRestricted ??= _data;
        public THandler Handler => _handler;
    }

    public class ComputingData<THandler, TData, TStruct>
        where THandler : ComputingHandler where TData : ComputingData<TStruct> where TStruct : struct
    {
#pragma warning disable 0649
        [SerializeField] private THandler _handler;
        [SerializeField] private TData[] _data;

        private TStruct[] _dataBaked;
        private IReadOnlyList<TStruct> _dataRestricted;
#pragma warning restore 0649

        public IReadOnlyList<TStruct> Data => _dataRestricted ??= _dataBaked;
        public THandler Handler => _handler;


        public void BakeData()
        {
            var flag = false;
            for (var i = 0; i < _data.Length && !flag; i++)
            {
                if (_data[i] != null) continue;
                
                flag = true;
#if UNITY_EDITOR
                Debug.LogError($"Empty {i} element of data array");
#endif
            }
            if (flag) return;

            _dataBaked = new TStruct[_data.Length];
            for (var i = 0; i < _data.Length; i++)
                _dataBaked[i] = _data[i].Data;
        }
    }

}

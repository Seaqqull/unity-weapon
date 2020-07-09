using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ComputingHandler : ScriptableObject { }

    public class ComputingData<T> : ScriptableObject 
        where T : struct
    {
        [SerializeField]
        public T Data;
    }

    public class ComputingObject<THandler, TObject>
    where THandler : ComputingHandler
    {
#pragma warning disable 0649
        [SerializeField] private THandler _handler;
        [SerializeField] private TObject[] _objects;

        private IReadOnlyList<TObject> _objectsRestricted;
#pragma warning restore 0649

        public THandler Handler
        {
            get { return this._handler; }
        }
        public IReadOnlyList<TObject> Objects
        {
            get
            {
                return this._objectsRestricted ??
                    (this._objectsRestricted = _objects);
            }
        }
    }

    public class ComputingData<THandler, TData>
    where THandler : ComputingHandler
    {
#pragma warning disable 0649
        [SerializeField] private THandler _handler;
        [SerializeField] private TData[] _data;

        private IReadOnlyList<TData> _dataRestricted;
#pragma warning restore 0649

        public THandler Handler
        {
            get { return this._handler; }
        }
        public IReadOnlyList<TData> Data
        {
            get
            {
                return this._dataRestricted ??
                    (this._dataRestricted = _data);
            }
        }

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

        public THandler Handler
        {
            get { return this._handler; }
        }
        public IReadOnlyList<TStruct> Data
        {
            get
            {
                return this._dataRestricted ??
                    (this._dataRestricted = _dataBaked);
            }
        }


        public void BakeData()
        {
            bool flag = false;
            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] == null)
                {
                    flag = true;
#if UNITY_EDITOR
                    Debug.LogError($"Empty {i} element of data array");
#endif
                }
            }
#if UNITY_EDITOR
            Debug.LogError("Data cannot be baked");
#endif
            if (flag) return;

            _dataBaked = new TStruct[_data.Length];

            for (int i = 0; i < _data.Length; i++)
            {
                _dataBaked[i] = _data[i].Data;
            }
        }
    }

}

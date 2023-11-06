using System.Collections.Generic;
using UnityEngine;


namespace Weapon.Computing
{
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
}
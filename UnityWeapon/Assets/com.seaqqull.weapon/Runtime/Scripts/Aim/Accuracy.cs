using UnityEngine;
using Weapon.Base;


namespace Weapons.Aiming
{
    public abstract class Accuracy : BaseMonoBehaviour, IAccuracy
    {
        [SerializeField] protected Transform _flow;
        [Header("Visualization")]
        [SerializeField] protected Color _shapeColor = new(0, 1, 0, 1);
        [SerializeField] protected Color _precisionColor = new(1, 1, 0, 1);

        public Transform Flow => _flow;


        protected abstract void OnDrawGizmos();

        protected abstract void MakeRandomDirection();
        protected abstract void ClearRandomDirection();
        

        public abstract Line[] CreateDirection();
    }
}
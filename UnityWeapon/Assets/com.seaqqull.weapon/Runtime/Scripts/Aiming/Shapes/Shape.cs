using UnityEngine;


namespace Weapons.Aiming.Shapes
{
    public abstract class Shape : ScriptableObject, IShape
    {
        private IShape _shapeImplementation;
        public abstract Region Property { get; }

        public virtual void DrawGizmos(Vector3 position, Vector3 forward, Quaternion rotation, Color shapeColor, Color precisionColor)
        {
            Gizmos.color = Property.Color;
            Gizmos.DrawLine(position, position + (forward * Property.Distance));
        }


        public abstract Vector3 CalculateVector();
        public abstract Vector3 Clamp(Vector3 point);
        public abstract Vector3 CalculateVector(Vector3 position, Quaternion rotation);
    }

    public abstract class Shape<T> : Shape
        where T : Region
    {
        [SerializeField] protected T _property;

        public override Region Property => _property;
    }
}
using UnityEngine;


namespace Weapons.Aiming.Shapes
{
    public interface IShape
    {
        Region Property { get; }


        void DrawGizmos(Vector3 position, Vector3 forward, Quaternion rotation, Color shapeColor, Color precisionColor);


        Vector3 CalculateVector();
        Vector3 Clamp(Vector3 point);
        Vector3 CalculateVector(Vector3 position, Quaternion rotation);
    }
}
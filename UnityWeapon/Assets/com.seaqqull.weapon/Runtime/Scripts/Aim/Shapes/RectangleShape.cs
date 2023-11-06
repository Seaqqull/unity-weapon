using Utilities.Methods;
using UnityEngine;


namespace Weapons.Aiming.Shapes
{
    [CreateAssetMenu(fileName = "RectangleShape", menuName = "Weapon/Aiming/Shapes/Rectangle", order = 0)]
    public class RectangleShape : Shape<RectangleRegion>
    {
        public override void DrawGizmos(Vector3 position, Vector3 forward, Quaternion rotation, Color shapeColor, Color precisionColor)
        {
            base.DrawGizmos(position, forward, rotation, shapeColor, precisionColor);

            DrawRectangleGizmo(position, rotation, 
                offset: forward * _property.Distance, 
                _property.Width, _property.Height,
                color: shapeColor);
            DrawRectangleGizmo(position, rotation, 
                offset: forward * _property.Distance,
                width: (_property.Width * (1 - _property.Precision)), 
                height: (_property.Height * (1 - _property.Precision)), 
                color: precisionColor);

            void DrawRectangleGizmo(Vector3 position, Quaternion rotation, Vector3 offset = new(), float width = 1, float height = 1, Color color = new())
            {
                var positionWithOffset = position + offset;
                var originalColor = Gizmos.color;
                Gizmos.color = color;

                var vertices = CreateRectangle(positionWithOffset, rotation,width, height);
                for (var i = 0; i < vertices.Length - 1; i++)
                    Gizmos.DrawLine(vertices[i], vertices[i + 1]);

                Gizmos.DrawLine(vertices[^1], vertices[0]);
                Gizmos.color = originalColor;
            }
            
            Vector3[] CreateRectangle(Vector3 position, Quaternion rotation, float width, float height)
            {
                var vertices = new Vector3[4];
                vertices[0] = position + (rotation * new Vector3(-width / 2.0f,  height / 2.0f, 0.0f));
                vertices[1] = position + (rotation * new Vector3( width / 2.0f,  height / 2.0f, 0.0f));
                vertices[2] = position + (rotation * new Vector3(width / 2.0f, -height / 2.0f, 0.0f));
                vertices[3] = position + (rotation * new Vector3( -width / 2.0f, -height / 2.0f, 0.0f));

                return vertices;
            }
        }


        public override Vector3 CalculateVector()
        {
            var sizeScale = 1 - _property.Precision;
            var halfHeight = _property.Height / 2 * sizeScale;
            var halfWidth = _property.Width / 2 * sizeScale;
            return new Vector3(Random.value.Map(0, 1, -halfWidth, halfWidth), Random.value.Map(0, 1, -halfHeight, halfHeight), 0);
        }

        public override Vector3 CalculateVector(Vector3 position, Quaternion rotation)
        {
            return position + (rotation * CalculateVector());
        }
    }
}
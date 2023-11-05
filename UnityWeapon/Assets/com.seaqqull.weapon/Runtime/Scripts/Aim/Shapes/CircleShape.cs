using UnityEngine;


namespace Weapons.Aiming.Shapes
{
    [CreateAssetMenu(fileName = "CircleShape", menuName = "Weapon/Aiming/Shapes/Circle", order = 0)]
    public class CircleShape : Shape<CircleRegion>
    {
        [SerializeField] private int _edgesCount = 10;


        public override void DrawGizmos(Vector3 position, Vector3 forward, Quaternion rotation, Color shapeColor, Color precisionColor)
        {
            base.DrawGizmos(position, forward, rotation, shapeColor, precisionColor);
            
            DrawCircleGizmo(position, rotation, 
                offset: forward * _property.Distance, 
                radius: _property.Radius,
                color: shapeColor);
            DrawCircleGizmo(position, rotation, 
                offset: forward * _property.Distance,
                radius: (_property.Radius * (1 - _property.Precision)), 
                color: precisionColor);
            
            void DrawCircleGizmo(Vector3 position, Quaternion rotation, Vector3 offset = new(), float radius = 0, Color color = new())
            {
                if (radius <= 0 && _edgesCount < 3) return;

                var positionWithOffset = position + offset;
                var originalColor = Gizmos.color;
                Gizmos.color = color;

                var vertices = CreateCircle(positionWithOffset, rotation, radius, _edgesCount);
                for (var i = 0; i < vertices.Length - 1; i++)
                    Gizmos.DrawLine(vertices[i], vertices[i + 1]);

                Gizmos.DrawLine(vertices[^1], vertices[0]);
                Gizmos.color = originalColor;
            }
            
            Vector3[] CreateCircle(Vector3 position, Quaternion rotation, float radius, int edgeCount)
            {
                var angleStep = (360.0f / edgeCount);
                var vertices = new Vector3[edgeCount];
            
                for (var i = 0; i < edgeCount; i++)
                {
                    var angle = i * angleStep;
                    var arcPoint = new Vector3(
                        Mathf.Sin(Mathf.Deg2Rad *  angle) * radius, 
                        Mathf.Cos(Mathf.Deg2Rad *  angle) * radius, 
                        0);

                    vertices[i] = position + rotation * arcPoint;
                }

                return vertices;
            }
        }
        
        public override Vector3 CalculateVector()
        {
            var rndPos = Random.insideUnitCircle * _property.Radius * (1 - _property.Precision);
            return new Vector3(rndPos.x, rndPos.y, 0);
        }

        public override Vector3 CalculateVector(Vector3 position, Quaternion rotation)
        {
            return position + (rotation * CalculateVector());
        }
    }
}
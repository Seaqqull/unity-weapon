using Random = UnityEngine.Random;
using UnityEngine;
using Utilities.Methods;
using Weapon.Base;


namespace Weapons.Aiming
{
    public class OldAccuracy : BaseMonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private float _bakeDistance;
        [SerializeField] private float _flowDistance = 1.0f;
        [Space]
        [SerializeField] private Region _begin;
        [SerializeField] private Region _end;
        [Header("Visualization")]
        [SerializeField] private int _edgeCount = 10;
        [SerializeField] private Color _bakeColor = new(0, 0, 0, 1);
        [SerializeField] private Color _flowColor = new(1, 1, 0, 1);
        [SerializeField] private CircleVisualization _beginVisualization;
        [SerializeField] private CircleVisualization _endVisualization;
        [SerializeReference] private IAccuracy _accuracy;
#if UNITY_EDITOR
        private Vector3 beginLine, endLine;
#endif

        public Transform Flow => _parent;
        public Region Begin => _begin;
        public Region End => _end;


        private void OnDrawGizmos()
        {
            if (_parent == null) return;

            // var errorFlag =
            //     DrawCircleGizmo(_parent.position, _parent.rotation, offset: _parent.forward * _bakeDistance, radius: _begin.Size, color: _beginVisualization.ColorRadius) &&
            //     DrawCircleGizmo(_parent.position, _parent.rotation, offset: _parent.forward * _bakeDistance, radius: (_begin.Size * (1 - _begin.Precision)), color: _beginVisualization.ColorAccuracy);
            // if (!errorFlag)
            //     Debug.LogWarning($"Wrong parameters ({nameof(Region.Size)}, {nameof(_edgeCount)})" +
            //                      " in begin circle, so it can't be displayed properly.", GameObj);
            //
            // errorFlag =
            //     DrawCircleGizmo(_parent.position, _parent.rotation, offset: _parent.forward * (_flowDistance + _bakeDistance), radius: _end.Size, color: _endVisualization.ColorRadius) &&
            //     DrawCircleGizmo(_parent.position, _parent.rotation, offset: _parent.forward * (_flowDistance + _bakeDistance), radius: (_end.Size * (1 - _end.Precision)), color: _endVisualization.ColorAccuracy);
            // if (!errorFlag)
            //     Debug.LogWarning($"Wrong parameters ({nameof(Region.Size)}, {nameof(_edgeCount)})" +
            //                      " in begin circle, so it can't be displayed properly.", GameObj);
            //
            // if (_parent != null)
            // {
            //     Gizmos.color = _bakeColor;
            //     Gizmos.DrawLine(_parent.position, _parent.position + _parent.forward * _bakeDistance);
            //     
            //     Gizmos.color = _flowColor;
            //     Gizmos.DrawLine(_parent.position + _parent.forward * _bakeDistance, _parent.position + _parent.forward * (_flowDistance + _bakeDistance));
            // }

#if UNITY_EDITOR
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(beginLine, endLine);
#endif
            bool DrawCircleGizmo(Vector3 position, Quaternion rotation, Vector3 offset = new(), float radius = 0, Color color = new())
            {
                if (radius <= 0 && _edgeCount < 3) return false;

                var positionWithOffset = position + offset;
                var originalColor = Gizmos.color;
                Gizmos.color = color;

                var vertices = CreateCircle(positionWithOffset, rotation, radius, _edgeCount);
                for (var i = 0; i < vertices.Length - 1; i++)
                    Gizmos.DrawLine(vertices[i], vertices[i + 1]);

                Gizmos.DrawLine(vertices[^1], vertices[0]);
                Gizmos.color = originalColor;

                return true;
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


#if UNITY_EDITOR
        [ContextMenu("New random direction")]
        private void MakeRandomDirection()
        {
            // beginLine = CalculateVector(
            //     _parent.position, 
            //     _parent.rotation, 
            //     _begin.Size * (1 - _begin.Precision));
            // endLine = CalculateVector(
            //     _parent.position + (_parent.forward * _flowDistance), 
            //     _parent.rotation, 
            //     _end.Size * (1 - _end.Precision));
        }
#endif

        private static float MapAccuracy(float from, float ratio) =>
            ratio >= 0.0f 
                ? ratio >= 0.5f ? ratio.Map(0.5f, 1.0f, from * 2.0f, 1.0f) 
                    : ratio.Map(0.0f, 0.5f, from, from * 2.0f)
                : ratio < -0.5f ? ratio.Map(-0.5f, -1.0f, from * 0.5f, 0.0f) 
                    : ratio.Map(0.0f, -0.5f, from, from * 0.5f);

        private static Vector3 CalculateVector(Vector3 position, Quaternion rotation, float radius)
        {
            var rndPos = Random.insideUnitCircle * radius;
            var shift = new Vector3(rndPos.x, rndPos.y, 0);

            return position + (rotation * shift);
        }
        

        public Line GetDirectionVector()
        {
            // var rotation = _parent.rotation;
            // var position = _parent.position;
            //
            // var end = CalculateVector(position + (_parent.forward * _flowDistance), rotation, _end.Size * (1 - _end.Precision));
            // var begin = CalculateVector(position, rotation, _begin.Size * (1 - _begin.Precision));
            //
            // return new Line(begin, (end - begin).normalized);
            return new Line(_parent.position, _parent.forward, 0.0f);
        }

        /// <summary>
        /// Sets begin and end accuracy.
        /// </summary>
        /// <param name="percent"> Global accuracy value.</param>
        /// <param name="aspectRatio"> Velue between 0 and 2. Relation between begin and end accuracy.</param>
        public void SetGlobalAccuracy(float percent, float aspectRatio)
        {
            aspectRatio = Mathf.Clamp(aspectRatio, -1.0f, 1.0f);
            percent = Mathf.Clamp01(percent);

            if (aspectRatio == 0.0f)
            {
                _begin.Precision = percent;
                _end.Precision = percent;
                return;
            }
            
            var absoluteAspectRatio = Mathf.Abs(aspectRatio);
            var mappedAccuracy = MapAccuracy(percent, absoluteAspectRatio);
            if (aspectRatio > 0.0f)
            {
                _begin.Precision = percent;
                _end.Precision = mappedAccuracy;
            }
            else
            {
                _begin.Precision = mappedAccuracy;
                _end.Precision = percent;
            }
        }
        
        public void SetGlobalAccuracyFromMiddle(float percent, float aspectRatio)
        {
            aspectRatio = Mathf.Clamp(aspectRatio, -1.0f, 1.0f);
            percent = Mathf.Clamp01(percent);

            if (aspectRatio == 0.0f)
            {
                _begin.Precision = percent;
                _end.Precision = percent;
                return;
            }

            var mappedEndAccuracy = MapAccuracy(percent, aspectRatio);
            var mappedStartAccuracy = MapAccuracy(percent, -aspectRatio);

            _begin.Precision = mappedStartAccuracy;
            _end.Precision = mappedEndAccuracy;
        }
    }
}
